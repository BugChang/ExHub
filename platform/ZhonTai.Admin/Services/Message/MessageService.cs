using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Domain.Message;
using ZhonTai.Admin.Services.Message.Dto;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;

namespace ZhonTai.Admin.Services.Message
{
    /// <summary>
    /// 消息服务
    /// </summary>
    [DynamicApi(Area = AdminConsts.AreaName)]
    public class MessageService : BaseService, IMessageService, IDynamicApi
    {
        private readonly Lazy<IMessageRepository> _messageRepository;

        public MessageService(Lazy<IMessageRepository> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="input">查询条件组合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PageOutput<MessageGetPageDto>> GetPageAsync(PageInput<MessageGetPageInput> input)
        {
            var list = await _messageRepository.Value.Select
                .Where(a => a.ReceiveUserId == User.Id)
                .WhereIf(input.Filter.IsRead.HasValue, a => a.IsRead == input.Filter.IsRead)
                .WhereIf(input.Filter.Title.NotNull(), p => p.Title.Contains(input.Filter.Title))
                .Count(out var total)
                .OrderByDescending(p => p.Id)
                .Page(input.CurrentPage, input.PageSize)
                .ToListAsync();

            var data = new PageOutput<MessageGetPageDto>
            {
                List = Mapper.Map<List<MessageGetPageDto>>(list),
                Total = total
            };

            return data;
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="id">消息Id</param>
        /// <returns></returns>
        public async Task<MessageDetailDto> GetAsync(long id)
        {
            var data = await _messageRepository.Value.GetAsync(id);
            data.IsRead = true;
            await _messageRepository.Value.UpdateAsync(data);
            return Mapper.Map<MessageDetailDto>(data);
        }

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<long> AddAsync(MessageAddInput input)
        {
            var entity = Mapper.Map<MessageEntity>(input);
            entity.CreatedTime = DateTime.Now;
            await _messageRepository.Value.InsertAsync(entity);
            return entity.Id;
        }
    }
}
