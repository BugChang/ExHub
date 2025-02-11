﻿using System.Threading.Tasks;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Services.File.Dto;
using ZhonTai.DynamicApi;
using ZhonTai.DynamicApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Domain.File.Dto;
using ZhonTai.Admin.Domain.File;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using ZhonTai.Admin.Core.Configs;
using OnceMi.AspNetCore.OSS;
using Microsoft.Extensions.Options;
using System.Linq;
using ZhonTai.Common.Files;
using ZhonTai.Common.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using ZhonTai.Admin.Core.Helpers;
using Microsoft.AspNetCore.Hosting;
using System.IO.Compression;

namespace ZhonTai.Admin.Services.File;

/// <summary>
/// 文件服务
/// </summary>
[Order(110)]
[DynamicApi(Area = AdminConsts.AreaName)]
public class FileService : BaseService, IFileService, IDynamicApi
{
    private IFileRepository _fileRepository => LazyGetRequiredService<IFileRepository>();
    private IOSSServiceFactory _oSSServiceFactory => LazyGetRequiredService<IOSSServiceFactory>();
    private OSSConfig _oSSConfig => LazyGetRequiredService<IOptions<OSSConfig>>().Value;
    private IHttpContextAccessor _httpContextAccessor => LazyGetRequiredService<IHttpContextAccessor>();

    public FileService()
    {

    }

    /// <summary>
    /// 查询分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PageOutput<FileGetPageOutput>> GetPageAsync(PageInput<FileGetPageDto> input)
    {
        var fileName = input.Filter?.FileName;

        var list = await _fileRepository.Select
        .WhereDynamicFilter(input.DynamicFilter)
        .WhereIf(fileName.NotNull(), a => a.FileName.Contains(fileName))
        .Count(out var total)
        .OrderByDescending(true, c => c.Id)
        .Page(input.CurrentPage, input.PageSize)
        .ToListAsync(a => new FileGetPageOutput { ProviderName = a.Provider.ToString() });

        var data = new PageOutput<FileGetPageOutput>()
        {
            List = list,
            Total = total
        };

        return data;
    }

    /// <summary>
    /// 查询单条
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<FileGetOutput> GetAsync(long id)
    {
        var file = await _fileRepository.Select
            .Where(a => a.Id == id)
            .ToOneAsync<FileGetOutput>();
        return file;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task DeleteAsync(FileDeleteInput input)
    {
        var file = await _fileRepository.GetAsync(input.Id);
        if (file == null)
        {
            return;
        }

        var shareFile = await _fileRepository.Where(a => a.Id != input.Id && a.LinkUrl == file.LinkUrl).AnyAsync();
        if (!shareFile)
        {
            if (file.Provider.HasValue)
            {
                var oSSService = _oSSServiceFactory.Create(file.Provider.ToString());
                var oSSOptions = _oSSConfig.OSSConfigs.Where(a => a.Enable && a.Provider == file.Provider).FirstOrDefault();
                var enableOss = oSSOptions != null && oSSOptions.Enable;
                if (enableOss)
                {
                    var filePath = Path.Combine(file.FileDirectory, file.SaveFileName + file.Extension).ToPath();
                    await oSSService.RemoveObjectAsync(file.BucketName, filePath);
                }
            }
            else
            {
                var env = LazyGetRequiredService<IWebHostEnvironment>();
                var filePath = Path.Combine(env.WebRootPath, file.FileDirectory, file.SaveFileName + file.Extension).ToPath();
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        await _fileRepository.DeleteAsync(file.Id);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="fileDirectory">文件目录</param>
    /// <param name="fileReName">文件重命名</param>
    /// <returns></returns>
    public async Task<FileEntity> UploadFileAsync([Required] IFormFile file, string fileDirectory = "", bool fileReName = true)
    {
        var localUploadConfig = _oSSConfig.LocalUploadConfig;

        var extention = Path.GetExtension(file.FileName).ToLower();
        var hasIncludeExtension = localUploadConfig.IncludeExtension?.Length > 0;
        if (hasIncludeExtension && !localUploadConfig.IncludeExtension.Contains(extention))
        {
            throw new Exception($"不允许上传{extention}文件格式");
        }
        var hasExcludeExtension = localUploadConfig.ExcludeExtension?.Length > 0;
        if (hasExcludeExtension && localUploadConfig.ExcludeExtension.Contains(extention))
        {
            throw new Exception($"不允许上传{extention}文件格式");
        }

        var fileLenth = file.Length;
        if (fileLenth > localUploadConfig.MaxSize)
        {
            throw new Exception($"文件大小不能超过{new FileSize(localUploadConfig.MaxSize)}");
        }

        var oSSOptions = _oSSConfig.OSSConfigs.Where(a => a.Enable && a.Provider == _oSSConfig.Provider).FirstOrDefault();
        var enableOss = oSSOptions != null && oSSOptions.Enable;
        var enableMd5 = enableOss ? oSSOptions.Md5 : localUploadConfig.Md5;
        var md5 = string.Empty;
        if (enableMd5)
        {
            md5 = MD5Encrypt.GetHash(file.OpenReadStream());
            var md5FileEntity = await _fileRepository.WhereIf(enableOss, a => a.Provider == oSSOptions.Provider).Where(a => a.Md5 == md5).FirstAsync();
            if (md5FileEntity != null)
            {
                var sameFileEntity = new FileEntity
                {
                    Provider = md5FileEntity.Provider,
                    BucketName = md5FileEntity.BucketName,
                    FileGuid = FreeUtil.NewMongodbId(),
                    SaveFileName = md5FileEntity.SaveFileName,
                    FileName = Path.GetFileNameWithoutExtension(file.FileName),
                    Extension = extention,
                    FileDirectory = md5FileEntity.FileDirectory,
                    Size = md5FileEntity.Size,
                    SizeFormat = md5FileEntity.SizeFormat,
                    LinkUrl = md5FileEntity.LinkUrl,
                    Md5 = md5,
                };
                sameFileEntity = await _fileRepository.InsertAsync(sameFileEntity);
                return sameFileEntity;
            }
        }

        if (fileDirectory.IsNull())
        {
            fileDirectory = localUploadConfig.Directory;
            if (localUploadConfig.DateTimeDirectory.NotNull())
            {
                fileDirectory = Path.Combine(fileDirectory, DateTime.Now.ToString(localUploadConfig.DateTimeDirectory)).ToPath();
            }
        }

        var fileSize = new FileSize(fileLenth);
        var fileEntity = new FileEntity
        {
            Provider = oSSOptions?.Provider,
            BucketName = oSSOptions?.BucketName,
            FileGuid = FreeUtil.NewMongodbId(),
            FileName = Path.GetFileNameWithoutExtension(file.FileName),
            Extension = extention,
            FileDirectory = fileDirectory,
            Size = fileSize.Size,
            SizeFormat = fileSize.ToString(),
            Md5 = md5
        };
        fileEntity.SaveFileName = fileReName ? fileEntity.FileGuid.ToString() : fileEntity.FileName;

        var filePath = Path.Combine(fileDirectory, fileEntity.SaveFileName + fileEntity.Extension).ToPath();
        var url = string.Empty;
        if (enableOss)
        {
            url = oSSOptions.Url;
            if (url.IsNull())
            {
                url = oSSOptions.Provider switch
                {
                    OSSProvider.Minio => $"{oSSOptions.Endpoint}/{oSSOptions.BucketName}",
                    OSSProvider.Aliyun => $"{oSSOptions.BucketName}.{oSSOptions.Endpoint}",
                    OSSProvider.QCloud => $"{oSSOptions.BucketName}-{oSSOptions.Endpoint}.cos.{oSSOptions.Region}.myqcloud.com",
                    OSSProvider.Qiniu => $"{oSSOptions.BucketName}.{oSSOptions.Region}.qiniucs.com",
                    OSSProvider.HuaweiCloud => $"{oSSOptions.BucketName}.{oSSOptions.Endpoint}",
                    _ => ""
                };

                if (url.IsNull())
                {
                    throw ResultOutput.Exception($"请配置{oSSOptions.Provider}的Url参数");
                }

                var urlProtocol = oSSOptions.IsEnableHttps ? "https" : "http";
                fileEntity.LinkUrl = $"{urlProtocol}://{url}/{filePath}";
            }
            else
            {
                fileEntity.LinkUrl = $"{url}/{filePath}";
            }
        }
        else
        {
            fileEntity.LinkUrl = $"{localUploadConfig.Url}/{filePath}";
        }

        if (enableOss)
        {
            var oSSService = _oSSServiceFactory.Create(_oSSConfig.Provider.ToString());
            await oSSService.PutObjectAsync(oSSOptions.BucketName, filePath, file.OpenReadStream());
        }
        else
        {
            var uploadHelper = LazyGetRequiredService<UploadHelper>();
            var env = LazyGetRequiredService<IWebHostEnvironment>();
            fileDirectory = Path.Combine(env.WebRootPath, fileDirectory).ToPath();
            if (!System.IO.Directory.Exists(fileDirectory))
            {
                System.IO.Directory.CreateDirectory(fileDirectory);
            }
            filePath = Path.Combine(env.WebRootPath, filePath).ToPath();
            await uploadHelper.SaveAsync(file, filePath);
        }

        fileEntity = await _fileRepository.InsertAsync(fileEntity);

        return fileEntity;
    }

    /// <summary>
    /// 上传多文件
    /// </summary>
    /// <param name="files">文件列表</param>
    /// <param name="fileDirectory">文件目录</param>
    /// <param name="fileReName">文件重命名</param>
    /// <returns></returns>
    public async Task<List<FileEntity>> UploadFilesAsync([Required] IFormFileCollection files, string fileDirectory = "", bool fileReName = true)
    {
        var fileList = new List<FileEntity>();
        foreach (var file in files)
        {
            fileList.Add(await UploadFileAsync(file, fileDirectory, fileReName));
        }
        return fileList;
    }

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> DownloadAsync(long id)
    {
        var file = await GetAsync(id);


        var env = LazyGetRequiredService<IWebHostEnvironment>();

        var filePath = Path.Combine(env.WebRootPath, file.FileDirectory, file.SaveFileName + file.Extension);
        // 检查文件是否存在
        if (!System.IO.File.Exists(filePath))
        {
            throw ResultOutput.Exception("文件不存在");
        }

        // 读取文件内容
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

        // 返回文件流
        return new FileContentResult(fileBytes, "application/octet-stream")
        {
            FileDownloadName = $"{file.FileName}{file.Extension}"
        };
    }

    /// <summary>
    /// 批量下载
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> BatchDownloadAsync(List<long> ids)
    {

        var filesToZip = new List<(string, string)>();

        var files = await _fileRepository.Select.Where(a => ids.Contains(a.Id)).ToListAsync();

        var env = LazyGetRequiredService<IWebHostEnvironment>();

        foreach (var file in files)
        {
            var filePath = Path.Combine(env.WebRootPath, file.FileDirectory, file.SaveFileName + file.Extension);
            // 检查文件是否存在
            if (!System.IO.File.Exists(filePath))
            {
                throw ResultOutput.Exception($"文件不存在:{file.FileName}");
            }
            filesToZip.Add((filePath, $"{file.FileName}{file.Extension}"));
        }

        using MemoryStream memoryStream = new MemoryStream();
        using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // 遍历要添加到 ZIP 文件的文件列表
            foreach ((string filePath, string newFileName) in filesToZip)
            {
                // 将文件添加到 ZIP 文件
                archive.CreateEntryFromFile(filePath, Path.GetFileName(newFileName));
            }
        }

        // 返回文件流
        return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
        {
            FileDownloadName = "output.zip"
        };
    }
}