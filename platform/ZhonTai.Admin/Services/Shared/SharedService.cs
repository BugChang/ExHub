using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.Shared;
using ZhonTai.Admin.Services.Auth;
using ZhonTai.Admin.Services.File;
using ZhonTai.Admin.Services.Shared.Input;
using ZhonTai.Admin.Services.Shared.Output;
using ZhonTai.Admin.Services.User;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.Shared
{
    /// <summary>
    /// 共享文件服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class SharedService(Lazy<ISharedRepository> sharedRepository, Lazy<IFileService> fileService, Lazy<IAuthService> authService)
        : BaseService, ISharedService, IDynamicApi
    {
        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<SharedListOutput>> GetPageAsync(PageInput<SharedListInput> input)
        {
            var permissions = await authService.Value.GetUserPermissionsAsync();

            var outside = permissions.Permissions.Contains("api:admin:home:outside");
            var inside = permissions.Permissions.Contains("api:admin:home:inside");

            var list = await sharedRepository.Value.Select.Include(a => a.File)
                .WhereIf(input.Filter.Key.NotNull(), a => a.File.FileName.Contains(input.Filter.Key))
                .WhereIf(outside && !inside, a => a.Scope == SharedScope.External || a.Scope == SharedScope.All)
                .WhereIf(!outside && inside, a => a.Scope == SharedScope.Internal || a.Scope == SharedScope.All)
                .WhereIf(!outside && !inside, a => a.Scope == SharedScope.All)
                .Count(out var total)
                .OrderByDescending(true, a => a.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync(a => new SharedListOutput()
                {
                    FileName = a.File.FileName,
                    FileId = a.File.Id,
                    FileSize = a.File.SizeFormat,
                    PublishTime = a.CreatedTime.Value,
                    FileExtension = a.File.Extension,
                    Id = a.Id,
                });
            var res = new PageOutput<SharedListOutput>()
            {
                List = list,
                Total = total
            };
            return res;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(long id)
        {
            await sharedRepository.Value.SoftDeleteAsync(m => m.Id == id);
        }



        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<long> UploadFileAsync(SharedUploadFileInput input)
        {
            var file = await fileService.Value.GetAsync(input.FileId);
            if (file == null)
            {
                throw ResultOutput.Exception("文件不存在");
            }

            var entity = Mapper.Map<SharedEntity>(new
            {
                input.Scope,
                input.FileId,
                File = file
            });
            await sharedRepository.Value.InsertAsync(entity);

            return entity.Id;
        }


    }
}
