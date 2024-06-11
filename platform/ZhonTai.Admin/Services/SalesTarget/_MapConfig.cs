using System;
using System.Linq;
using Mapster;
using Microsoft.Graph;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Domain.SalesTarget;
using ZhonTai.Admin.Services.SalesTarget.Dto;
using DateTime = System.DateTime;

namespace ZhonTai.Admin.Services.SalesTarget
{
    /// <summary>
    /// 映射配置
    /// </summary>
    public class MapConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            Func<SalesTargetEntity, EffectiveStatus> calcStatusFunc = src =>
            {
                if (src.Status == SalesTargetStatus.InActive || DateTime.Now > src.ExpirationDate)
                {
                    return EffectiveStatus.InActive;
                }

                if (DateTime.Now < src.EffectiveDate)
                {
                    return EffectiveStatus.NotActive;
                }

                return EffectiveStatus.Active;
            };

            config
                .NewConfig<SalesTargetEntity, SalesTargetGetPageOutput>()
                .Map(dest => dest.FileName, src => src.File.FileName)
                .Map(dest => dest.FileId, src => src.File.Id)
                .Map(dest => dest.Status, src => calcStatusFunc(src))
                .Map(dest => dest.TotalCount, src => src.Items.Sum(a => a.Count));
        }
    }
}
