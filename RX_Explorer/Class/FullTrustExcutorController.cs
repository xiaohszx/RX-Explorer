﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace RX_Explorer.Class
{
    /// <summary>
    /// 用于启动具备完全权限的附加程序的控制器
    /// </summary>
    public sealed class FullTrustExcutorController : IDisposable
    {
        private const string ExcuteType_RunExe = "Excute_RunExe";

        private const string ExcuteType_Quicklook = "Excute_Quicklook";

        private const string ExcuteType_Check_Quicklook = "Excute_Check_QuicklookIsAvaliable";

        private const string ExcuteType_Get_Associate = "Excute_Get_Associate";

        private const string ExcuteType_Get_RecycleBinItems = "Excute_Get_RecycleBinItems";

        private const string ExcuteType_Exit = "Excute_Exit";

        private const string ExcuteType_EmptyRecycleBin = "Excute_Empty_RecycleBin";

        private const string ExcuteType_Copy = "Excute_Copy";

        private const string ExcuteType_Move = "Excute_Move";

        private const string ExcuteType_Delete = "Excute_Delete";

        private const string ExcuteAuthority_Normal = "Normal";

        private const string ExcuteAuthority_Administrator = "Administrator";

        private volatile static FullTrustExcutorController Instance;

        private static readonly object locker = new object();

        private bool IsConnected = false;

        private AppServiceConnection Connection;

        public static FullTrustExcutorController Current
        {
            get
            {
                lock (locker)
                {
                    return Instance ??= new FullTrustExcutorController();
                }
            }
        }

        private FullTrustExcutorController()
        {
            Connection = new AppServiceConnection
            {
                AppServiceName = "CommunicateService",
                PackageFamilyName = Package.Current.Id.FamilyName
            };

            Connection.ServiceClosed += Connection_ServiceClosed;
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            IsConnected = false;
            Connection?.Dispose();
            Connection = null;
        }

        private async Task<bool> TryConnectToFullTrustExutor()
        {
            if (IsConnected)
            {
                return true;
            }

            try
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();

                return (await Connection.OpenAsync()) == AppServiceConnectionStatus.Success ? (IsConnected = true) : (IsConnected = false);
            }
            catch
            {
                return IsConnected = false;
            }
        }

        /// <summary>
        /// 启动指定路径的程序
        /// </summary>
        /// <param name="Path">程序路径</param>
        /// <returns></returns>
        public async Task RunAsync(string Path)
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(false))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_RunExe},
                    {"ExcutePath",Path },
                    {"ExcuteParameter",string.Empty},
                    {"ExcuteAuthority", ExcuteAuthority_Normal}
                };

                await Connection.SendMessageAsync(Value);
            }
        }

        /// <summary>
        /// 启动指定路径的程序，并传递指定的参数
        /// </summary>
        /// <param name="Path">程序路径</param>
        /// <param name="Parameter">传递的参数</param>
        /// <returns></returns>
        public async Task RunAsync(string Path, string Parameter)
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(false))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_RunExe},
                    {"ExcutePath",Path },
                    {"ExcuteParameter",Parameter},
                    {"ExcuteAuthority", ExcuteAuthority_Normal}
                };

                await Connection.SendMessageAsync(Value);
            }
        }

        /// <summary>
        /// 使用管理员权限启动指定路径的程序
        /// </summary>
        /// <param name="Path">程序路径</param>
        /// <returns></returns>
        public async Task RunAsAdministratorAsync(string Path)
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(false))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_RunExe},
                    {"ExcutePath",Path },
                    {"ExcuteParameter",string.Empty},
                    {"ExcuteAuthority", ExcuteAuthority_Administrator}
                };

                await Connection.SendMessageAsync(Value);
            }
        }

        /// <summary>
        /// 使用管理员权限启动指定路径的程序，并传递指定的参数
        /// </summary>
        /// <param name="Path">程序路径</param>
        /// <param name="Parameter">传递的参数</param>
        /// <returns></returns>
        public async Task RunAsAdministratorAsync(string Path, string Parameter)
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(false))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_RunExe},
                    {"ExcutePath",Path },
                    {"ExcuteParameter",Parameter},
                    {"ExcuteAuthority", ExcuteAuthority_Administrator}
                };

                await Connection.SendMessageAsync(Value);
            }
        }

        public async Task ViewWithQuicklookAsync(string Path)
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(false))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_Quicklook},
                    {"ExcutePath",Path }
                };

                await Connection.SendMessageAsync(Value);
            }
        }

        public async Task<bool> CheckQuicklookIsAvaliableAsync()
        {
            try
            {
                if (await TryConnectToFullTrustExutor().ConfigureAwait(false))
                {
                    ValueSet Value = new ValueSet
                    {
                        {"ExcuteType", ExcuteType_Check_Quicklook}
                    };

                    AppServiceResponse Reponse = await Connection.SendMessageAsync(Value);
                    if (Reponse.Status == AppServiceResponseStatus.Success && !Reponse.Message.ContainsKey("Error"))
                    {
                        return Convert.ToBoolean(Reponse.Message["Check_QuicklookIsAvaliable_Result"]);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetAssociateFromPathAsync(string Path)
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(false))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_Get_Associate},
                    {"ExcutePath", Path}
                };

                AppServiceResponse Reponse = await Connection.SendMessageAsync(Value);
                if (Reponse.Status == AppServiceResponseStatus.Success && !Reponse.Message.ContainsKey("Error"))
                {
                    return Convert.ToString(Reponse.Message["Associate_Result"]);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task EmptyRecycleBinAsync()
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(true))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_EmptyRecycleBin},
                };

                await Connection.SendMessageAsync(Value);
            }
        }

        public async Task<List<FileSystemStorageItem>> GetRecycleBinItemsAsync()
        {
            if (await TryConnectToFullTrustExutor().ConfigureAwait(true))
            {
                ValueSet Value = new ValueSet
                {
                    {"ExcuteType", ExcuteType_Get_RecycleBinItems},
                };

                AppServiceResponse Reponse = await Connection.SendMessageAsync(Value);
                if (Reponse.Status == AppServiceResponseStatus.Success && !Reponse.Message.ContainsKey("Error"))
                {
                    List<Dictionary<string, string>> Items = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(Convert.ToString(Reponse.Message["RecycleBinItems_Json_Result"]));
                    List<FileSystemStorageItem> Result = new List<FileSystemStorageItem>(Items.Count);

                    foreach (Dictionary<string, string> PropertyDic in Items)
                    {
                        FileSystemStorageItem Item = WIN_Native_API.GetStorageItems(PropertyDic["ActualPath"]).FirstOrDefault();
                        Item.SetAsRecycleItem(PropertyDic["OriginPath"], DateTime.FromBinary(Convert.ToInt64(PropertyDic["CreateTime"])));
                        Result.Add(Item);
                    }

                    return Result;
                }
                else
                {
                    return new List<FileSystemStorageItem>(0);
                }
            }
            else
            {
                return new List<FileSystemStorageItem>(0);
            }
        }

        public void Dispose()
        {
            ValueSet Value = new ValueSet
            {
                {"ExcuteType", ExcuteType_Exit},
            };

            if (IsConnected)
            {
                Connection.SendMessageAsync(Value).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            Connection?.Dispose();
            Connection = null;
        }
    }
}
