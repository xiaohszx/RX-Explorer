﻿using MySql.Data.MySqlClient;
using NetworkAccess;
using SQLConnectionPoolProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace RX_Explorer.Class
{
    /// <summary>
    /// 提供对MySQL数据库的访问支持
    /// </summary>
    public sealed class MySQL : IDisposable
    {
        private static MySQL Instance;

        private bool IsDisposed = false;

        private AutoResetEvent ConnectionLocker;

        private SQLConnectionPool<MySqlConnection> ConnectionPool;

        /// <summary>
        /// 提供对MySQL实例的访问
        /// </summary>
        public static MySQL Current
        {
            get
            {
                lock (SyncRootProvider.SyncRoot)
                {
                    return Instance ??= new MySQL();
                }
            }
        }

        /// <summary>
        /// 初始化MySQL实例
        /// </summary>
        private MySQL()
        {
            ConnectionLocker = new AutoResetEvent(true);
            using (SecureString Secure = SecureAccessProvider.GetMySQLAccessCredential(Package.Current))
            {
                IntPtr Bstr = Marshal.SecureStringToBSTR(Secure);
                string AccessCredential = Marshal.PtrToStringBSTR(Bstr);

                try
                {
                    ConnectionPool = new SQLConnectionPool<MySqlConnection>($"{AccessCredential}CharSet=utf8;Database=FeedBackDataBase;", 2, 1);
                }
                finally
                {
                    Marshal.ZeroFreeBSTR(Bstr);
                    unsafe
                    {
                        fixed (char* ClearPtr = AccessCredential)
                        {
                            for (int i = 0; i < AccessCredential.Length; i++)
                            {
                                ClearPtr[i] = '\0';
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从数据库连接池中获取连接对象
        /// </summary>
        /// <returns></returns>
        public Task<SQLConnection> GetConnectionFromPoolAsync()
        {
            return Task.Run(() =>
            {
                ConnectionLocker.WaitOne();

                SQLConnection Connection = ConnectionPool.GetConnectionFromDataBasePoolAsync().Result;

                #region MySQL数据库存储过程和触发器初始化代码，仅首次运行时需要
                //if (Connection.IsConnected)
                //{
                //    StringBuilder Builder = new StringBuilder();
                //    Builder.AppendLine("Create Table If Not Exists FeedBackTable (UserName Text Not Null, Title Text Not Null, Suggestion Text Not Null, LikeNum Text Not Null, DislikeNum Text Not Null, UserID Text Not Null, GUID Text Not Null);")
                //           .AppendLine("Create Table If Not Exists VoteRecordTable (UserID Text Not Null, GUID Text Not Null, Behavior Text Not Null);")

                //           .AppendLine("Drop Trigger RemoveVoteRecordTrigger;")
                //           .AppendLine("Create Trigger RemoveVoteRecordTrigger After Delete On FeedBackTable For Each Row Delete From VoteRecordTable Where GUID=old.GUID;")

                //           .AppendLine("Drop Procedure If Exists GetFeedBackProcedure;")
                //           .AppendLine("Create Procedure GetFeedBackProcedure(IN Para Text)")
                //           .AppendLine("Begin")
                //           .AppendLine("Declare EndSignal int Default 0;")
                //           .AppendLine("Declare P1 Text;")
                //           .AppendLine("Declare P2 Text;")
                //           .AppendLine("Declare P3 Text;")
                //           .AppendLine("Declare P4 Text;")
                //           .AppendLine("Declare P5 Text;")
                //           .AppendLine("Declare P6 Text;")
                //           .AppendLine("Declare P7 Text;")
                //           .AppendLine("Declare P8 Text;")
                //           .AppendLine("Declare RowData Cursor For Select * From FeedBackTable;")
                //           .AppendLine("Declare Continue Handler For Not Found Set EndSignal=1;")
                //           .AppendLine("Drop Table If Exists DataTemporary;")
                //           .AppendLine("Create Temporary Table DataTemporary (UserName Text, Title Text, Suggestion Text, LikeNum Text, DislikeNum Text, UserID Text, GUID Text, Behavior Text);")
                //           .AppendLine("Open RowData;")
                //           .AppendLine("Fetch RowData Into P1,P2,P3,P4,P5,P6,P7;")
                //           .AppendLine("While EndSignal<>1 Do")
                //           .AppendLine("If (Select Count(*) From VoteRecordTable Where UserID=Para And GUID=P7) <> 0")
                //           .AppendLine("Then")
                //           .AppendLine("Select Behavior Into P8 From VoteRecordTable Where UserID=Para And GUID=P7;")
                //           .AppendLine("Else")
                //           .AppendLine("Set P8 = 'NULL';")
                //           .AppendLine("End If;")
                //           .AppendLine("Insert Into DataTemporary Values (P1,P2,P3,P4,P5,P6,P7,P8);")
                //           .AppendLine("Fetch RowData Into P1,P2,P3,P4,P5,P6,P7;")
                //           .AppendLine("End While;")
                //           .AppendLine("Close RowData;")
                //           .AppendLine("Select * From DataTemporary;")
                //           .AppendLine("End;")

                //           .AppendLine("Drop Procedure If Exists UpdateFeedBackVoteProcedure;")
                //           .AppendLine("Create Procedure UpdateFeedBackVoteProcedure(IN LNum Text,IN DNum Text,IN UID Text,IN GID Text,IN Beh Text)")
                //           .AppendLine("Begin")
                //           .AppendLine("Update FeedBackTable Set LikeNum=LNum, DislikeNum=DNum Where GUID=GID;")
                //           .AppendLine("If (Select Count(*) From VoteRecordTable Where UserID=UID And GUID=GID) <> 0")
                //           .AppendLine("Then")
                //           .AppendLine("If Beh <> '='")
                //           .AppendLine("Then")
                //           .AppendLine("Update VoteRecordTable Set Behavior=Beh Where UserID=UID And GUID=GID;")
                //           .AppendLine("Else")
                //           .AppendLine("Delete From VoteRecordTable Where UserID=UID And GUID=GID;")
                //           .AppendLine("End If;")
                //           .AppendLine("Else")
                //           .AppendLine("If Beh <> '='")
                //           .AppendLine("Then")
                //           .AppendLine("Insert Into VoteRecordTable Values (UID,GID,Beh);")
                //           .AppendLine("End If;")
                //           .AppendLine("End If;")
                //           .AppendLine("End;");
                //    using (MySqlCommand Command = Connection.CreateDbCommandFromConnection<MySqlCommand>(Builder.ToString()))
                //    {
                //        _ = Command.ExecuteNonQuery();
                //    }
                //}
                #endregion

                ConnectionLocker.Set();

                return Connection;
            });
        }

        /// <summary>
        /// 获取所有反馈对象
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<FeedBackItem> GetAllFeedBackAsync()
        {
            using (SQLConnection Connection = await GetConnectionFromPoolAsync().ConfigureAwait(false))
            {
                if (Connection.IsConnected)
                {
                    using (MySqlCommand Command = Connection.CreateDbCommandFromConnection<MySqlCommand>("GetFeedBackProcedure", CommandType.StoredProcedure))
                    {
                        _ = Command.Parameters.AddWithValue("Para", ApplicationData.Current.LocalSettings.Values["SystemUserID"].ToString());
                        using (DbDataReader Reader = await Command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await Reader.ReadAsync().ConfigureAwait(false))
                            {
                                if (Reader["Behavior"].ToString() != "NULL")
                                {
                                    yield return new FeedBackItem(Reader["UserName"].ToString(), Reader["Title"].ToString(), Reader["Suggestion"].ToString(), Reader["LikeNum"].ToString(), Reader["DislikeNum"].ToString(), Reader["UserID"].ToString(), Reader["GUID"].ToString(), Reader["Behavior"].ToString());
                                }
                                else
                                {
                                    yield return new FeedBackItem(Reader["UserName"].ToString(), Reader["Title"].ToString(), Reader["Suggestion"].ToString(), Reader["LikeNum"].ToString(), Reader["DislikeNum"].ToString(), Reader["UserID"].ToString(), Reader["GUID"].ToString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// 更新反馈对象的投票信息
        /// </summary>
        /// <param name="Item">反馈对象</param>
        /// <returns></returns>
        public async Task<bool> UpdateFeedBackVoteAsync(FeedBackItem Item)
        {
            if (Item != null)
            {
                using (SQLConnection Connection = await GetConnectionFromPoolAsync().ConfigureAwait(false))
                {
                    if (Connection.IsConnected)
                    {
                        try
                        {
                            using (MySqlCommand Command = Connection.CreateDbCommandFromConnection<MySqlCommand>("UpdateFeedBackVoteProcedure", CommandType.StoredProcedure))
                            {
                                _ = Command.Parameters.AddWithValue("LNum", Item.LikeNum);
                                _ = Command.Parameters.AddWithValue("DNum", Item.DislikeNum);
                                _ = Command.Parameters.AddWithValue("Beh", Item.UserVoteAction);
                                _ = Command.Parameters.AddWithValue("GID", Item.GUID);
                                _ = Command.Parameters.AddWithValue("UID", ApplicationData.Current.LocalSettings.Values["SystemUserID"].ToString());
                                _ = await Command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(Item), "Parameter could not be null");
            }
        }

        /// <summary>
        /// 更新反馈对象的标题和建议内容
        /// </summary>
        /// <param name="Title">标题</param>
        /// <param name="Suggestion">建议</param>
        /// <param name="Guid">唯一标识</param>
        /// <returns></returns>
        public async Task<bool> UpdateFeedBackAsync(string Title, string Suggestion, string Guid)
        {
            using (SQLConnection Connection = await GetConnectionFromPoolAsync().ConfigureAwait(false))
            {
                if (Connection.IsConnected)
                {
                    try
                    {
                        using (MySqlCommand Command = Connection.CreateDbCommandFromConnection<MySqlCommand>("Update FeedBackTable Set Title=@NewTitle, Suggestion=@NewSuggestion Where GUID=@GUID"))
                        {
                            _ = Command.Parameters.AddWithValue("@NewTitle", Title);
                            _ = Command.Parameters.AddWithValue("@NewSuggestion", Suggestion);
                            _ = Command.Parameters.AddWithValue("@GUID", Guid);
                            _ = Command.ExecuteNonQuery();
                        }
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除反馈内容
        /// </summary>
        /// <param name="Item">反馈对象</param>
        /// <returns></returns>
        public async Task<bool> DeleteFeedBackAsync(FeedBackItem Item)
        {
            if (Item != null)
            {
                using (SQLConnection Connection = await GetConnectionFromPoolAsync().ConfigureAwait(false))
                {
                    if (Connection.IsConnected)
                    {
                        try
                        {
                            using (MySqlCommand Command = Connection.CreateDbCommandFromConnection<MySqlCommand>("Delete From FeedBackTable Where GUID=@GUID"))
                            {
                                _ = Command.Parameters.AddWithValue("@GUID", Item.GUID);
                                _ = Command.ExecuteNonQuery();
                            }

                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(Item), "Parameter could not be null");
            }
        }

        /// <summary>
        /// 提交反馈内容
        /// </summary>
        /// <param name="Item">反馈对象</param>
        /// <returns></returns>
        public async Task<bool> SetFeedBackAsync(FeedBackItem Item)
        {
            if (Item != null)
            {
                using (SQLConnection Connection = await GetConnectionFromPoolAsync().ConfigureAwait(false))
                {
                    if (Connection.IsConnected)
                    {
                        try
                        {
                            using (MySqlCommand Command = Connection.CreateDbCommandFromConnection<MySqlCommand>("Insert Into FeedBackTable Values (@UserName,@Title,@Suggestion,@Like,@Dislike,@UserID,@GUID)"))
                            {
                                _ = Command.Parameters.AddWithValue("@UserName", Item.UserName);
                                _ = Command.Parameters.AddWithValue("@Title", Item.Title);
                                _ = Command.Parameters.AddWithValue("@Suggestion", Item.Suggestion);
                                _ = Command.Parameters.AddWithValue("@Like", Item.LikeNum);
                                _ = Command.Parameters.AddWithValue("@Dislike", Item.DislikeNum);
                                _ = Command.Parameters.AddWithValue("@UserID", Item.UserID);
                                _ = Command.Parameters.AddWithValue("@GUID", Item.GUID);
                                _ = Command.ExecuteNonQuery();
                            }
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(Item), "Parameter could not be null");
            }
        }

        /// <summary>
        /// 调用此方法以完全释放MySQL的资源
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                ConnectionPool.Dispose();
                ConnectionLocker?.Dispose();
                ConnectionPool = null;
                ConnectionLocker = null;
                Instance = null;

                GC.SuppressFinalize(this);
            }
        }

        ~MySQL()
        {
            Dispose();
        }
    }
}
