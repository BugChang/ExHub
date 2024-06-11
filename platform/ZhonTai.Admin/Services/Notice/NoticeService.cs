using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.File;
using ZhonTai.Admin.Domain.Notice;
using ZhonTai.Admin.Domain.Order;
using ZhonTai.Admin.Services.Auth;
using ZhonTai.Admin.Services.Notice.Input;
using ZhonTai.Admin.Services.Notice.Output;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.Notice
{
    /// <summary>
    /// 公告服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class NoticeService : BaseService, INoticeService, IDynamicApi
    {
        private INoticeRepository NoticeRepository => LazyGetRequiredService<INoticeRepository>();

        private INoticeFileRepository NoticeFileRepository => LazyGetRequiredService<INoticeFileRepository>();

        private INoticeReadRepository NoticeReadRepository => LazyGetRequiredService<INoticeReadRepository>();

        private IFileRepository FileRepository => LazyGetRequiredService<IFileRepository>();

        private IOrderRepository OrderService => LazyGetRequiredService<IOrderRepository>();

        private IAuthService AuthService => LazyGetRequiredService<IAuthService>();


        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<NoticeListOutput>> GetPageAsync(PageInput<NoticeGetListInput> input)
        {
            var permissions = await AuthService.GetUserPermissionsAsync();

            var outside = permissions.Permissions.Contains("api:admin:home:outside");
            var inside = permissions.Permissions.Contains("api:admin:home:inside");

            var currentUser = OrderService.User;
            var data = await NoticeRepository.Orm.Select<NoticeEntity, NoticeReadEntity>()
                .LeftJoin((T, b) => T.Id == b.NoticeId && b.UserId == currentUser.Id)
                .WhereIf(input.Filter.Key.NotNull(), (T, b) => T.Title.Contains(input.Filter.Key) || T.Content.Contains(input.Filter.Key))
                .WhereIf(outside && !inside, a => a.t1.Scope == SharedScope.External || a.t1.Scope == SharedScope.All)
                .WhereIf(!outside && inside, a => a.t1.Scope == SharedScope.Internal || a.t1.Scope == SharedScope.All)
                .WhereIf(!outside && !inside, a => a.t1.Scope == SharedScope.All)
                .Count(out var total)
                .OrderByDescending((T, b) => T.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync((T, b) => new NoticeListOutput
                {
                    CreatedUserRealName = T.CreatedUserRealName,
                    ModifiedTime = T.ModifiedTime,
                    ModifiedUserName = T.ModifiedUserName,
                    ModifiedUserRealName = T.ModifiedUserRealName,
                    Scope = T.Scope,
                    Content = T.Content,
                    Title = T.Title,
                    CreatedTime = T.CreatedTime,
                    CreatedUserName = T.CreatedUserName,
                    Id = T.Id,
                    OrgName = T.OrgName,
                    IsRead = b != null ? true : false,
                });

            var res = new PageOutput<NoticeListOutput>()
            {
                List = data,
                Total = total
            };
            return res;
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NoticeDetailOutput> GetAsync(long id)
        {
            var currentUser = OrderService.User;
            var notice = await NoticeRepository.Select
                .Where(T => T.Id == id)
                .IncludeMany(a => a.Files).FirstAsync<NoticeEntity>();
            //查询公告当前用户是否已读
            var noticeRead = await NoticeReadRepository.Select
                .Where(T => T.UserId == currentUser.Id && T.NoticeId == id)
                .FirstAsync<NoticeReadEntity>();
            //如果查不到则插入一条记录标记为已读
            if (noticeRead == null)
            {
                NoticeReadEntity readEntity = new NoticeReadEntity()
                {
                    NoticeId = id,
                    UserId = currentUser.Id
                };
                await NoticeReadRepository.InsertAsync(readEntity);
            }

            var fileIds = notice.Files.Select(T => T.FileId).ToList();
            NoticeDetailOutput res = new NoticeDetailOutput()
            {
                Content = notice.Content,
                Title = notice.Title,
                Scope = notice.Scope,
                Id = notice.Id,
                OrgName = notice.OrgName,
                ModifiedTime = notice.ModifiedTime ?? notice.CreatedTime,
                CreatedTime = notice.CreatedTime,
                CreatedUserName = notice.CreatedUserName,
                ReadCreatedTime = noticeRead?.CreatedTime,
            };
            res.files = await FileRepository.Select.Where(T => fileIds.Contains(T.Id))
                .ToListAsync(T => new FileDetail
                {
                    FileId = T.Id
                });
            return res;
        }

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(long id)
        {
            await NoticeRepository.SoftDeleteAsync(m => m.Id == id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<long> AddAsync(NoticeAddInput input)
        {
            var entity = Mapper.Map<NoticeEntity>(input);
            await NoticeRepository.InsertAsync(entity);
            List<NoticeFileEntity> files = new List<NoticeFileEntity>();
            foreach (var item in input.FileIds)
            {
                files.Add(new NoticeFileEntity
                {
                    FileId = item,
                    NoticeId = entity.Id
                });
            }
            if (files.Count > 0)
            {
                await NoticeFileRepository.InsertAsync(files);
            }
            return entity.Id;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<long> UpdateAsync(NoticeUpdInput input)
        {
            var entity = Mapper.Map<NoticeEntity>(input);
            await NoticeRepository.UpdateAsync(entity);
            List<NoticeFileEntity> files = new List<NoticeFileEntity>();
            foreach (var item in input.FileIds)
            {
                files.Add(new NoticeFileEntity
                {
                    FileId = item,
                    NoticeId = entity.Id
                });
            }
            await NoticeFileRepository.DeleteAsync(T => T.NoticeId == entity.Id);
            if (files.Count > 0)
            {
                await NoticeFileRepository.InsertAsync(files);
            }
            return entity.Id;
        }
    }
}
