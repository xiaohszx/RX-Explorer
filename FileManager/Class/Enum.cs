﻿using System;

namespace FileManager.Class
{
    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }

    public enum SortTarget
    {
        Name = 0,
        Type = 1,
        ModifiedTime = 2,
        Size = 4
    }

    /// <summary>
    /// AES加密密钥长度枚举
    /// </summary>
    public enum KeySize
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// AES-128bit
        /// </summary>
        AES128 = 128,

        /// <summary>
        /// AES-256bit
        /// </summary>
        AES256 = 256
    }

    /// <summary>
    /// 压缩等级枚举
    /// </summary>
    public enum CompressionLevel
    {
        /// <summary>
        /// 最大
        /// </summary>
        Max = 9,

        /// <summary>
        /// 标准
        /// </summary>
        Standard = 4,

        /// <summary>
        /// 仅打包
        /// </summary>
        PackageOnly = 0
    }

    /// <summary>
    /// 图片滤镜类型
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// 原图
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 反色滤镜
        /// </summary>
        Invert = 1,
        /// <summary>
        /// 灰度滤镜
        /// </summary>
        Gray = 2,
        /// <summary>
        /// 二值化滤镜
        /// </summary>
        Threshold = 4,
        /// <summary>
        /// 素描滤镜
        /// </summary>
        Sketch = 8,
        /// <summary>
        /// 高斯模糊滤镜
        /// </summary>
        GaussianBlur = 16,
        /// <summary>
        /// 怀旧滤镜
        /// </summary>
        Sepia = 32,
        /// <summary>
        /// 马赛克滤镜
        /// </summary>
        Mosaic = 64,
        /// <summary>
        /// 油画滤镜
        /// </summary>
        OilPainting = 128
    }

    /// <summary>
    /// 提供对快速启动项状态或类型的枚举
    /// </summary>
    public enum QuickStartType
    {
        /// <summary>
        /// 应用区域的快速启动项
        /// </summary>
        Application = 1,
        /// <summary>
        /// 网站区域的快速启动项
        /// </summary>
        WebSite = 2,
        /// <summary>
        /// 指示此项用于更新应用区域
        /// </summary>
        UpdateApp = 4,
        /// <summary>
        /// 指示此向用于更新网站区域
        /// </summary>
        UpdateWeb = 8
    }

    /// <summary>
    /// Windows Hello授权状态
    /// </summary>
    public enum AuthenticatorState
    {
        /// <summary>
        /// 注册成功
        /// </summary>
        RegisterSuccess = 0,
        /// <summary>
        /// 用户取消
        /// </summary>
        UserCanceled = 1,
        /// <summary>
        /// 凭据丢失
        /// </summary>
        CredentialNotFound = 2,
        /// <summary>
        /// 未知错误
        /// </summary>
        UnknownError = 4,
        /// <summary>
        /// 系统不支持Windows Hello
        /// </summary>
        WindowsHelloUnsupport = 8,
        /// <summary>
        /// 授权通过
        /// </summary>
        VerifyPassed = 16,
        /// <summary>
        /// 授权失败
        /// </summary>
        VerifyFailed = 32,
        /// <summary>
        /// 用户未注册
        /// </summary>
        UserNotRegistered = 64
    }

    /// <summary>
    /// 语言枚举
    /// </summary>
    public enum LanguageEnum
    {
        /// <summary>
        /// 界面使用中文
        /// </summary>
        Chinese = 1,
        /// <summary>
        /// 界面使用英文
        /// </summary>
        English = 2
    }

    /// <summary>
    /// 反馈更新的类型
    /// </summary>
    public enum FeedBackUpdateType
    {
        /// <summary>
        /// 新增支持
        /// </summary>
        AddLike = 0,
        /// <summary>
        /// 删除支持
        /// </summary>
        DelLike = 1,
        /// <summary>
        /// 新增反对
        /// </summary>
        AddDislike = 2,
        /// <summary>
        /// 删除反对
        /// </summary>
        DelDislike = 4
    }

    /// <summary>
    /// 背景图片类型的枚举
    /// </summary>
    public enum BackgroundBrushType
    {
        /// <summary>
        /// 使用亚克力背景
        /// </summary>
        Acrylic = 0,

        /// <summary>
        /// 使用图片背景
        /// </summary>
        Picture = 1,

        /// <summary>
        /// 使用纯色背景
        /// </summary>
        SolidColor = 2,
    }

    /// <summary>
    /// 指定文件夹和库是自带还是用户固定
    /// </summary>
    public enum LibrarySource
    {
        /// <summary>
        /// 自带库
        /// </summary>
        SystemBase = 0,
        /// <summary>
        /// 用户自定义库
        /// </summary>
        UserCustom = 1
    }

    [Flags]
    public enum ItemFilter
    {
        File = 1,
        Folder = 2
    }
}
