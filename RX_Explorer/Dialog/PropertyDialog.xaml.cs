﻿using RX_Explorer.Class;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;

namespace RX_Explorer.Dialog
{
    public sealed partial class PropertyDialog : QueueContentDialog, INotifyPropertyChanged
    {
        public string FileName { get; private set; }

        public string FileType { get; private set; }

        public string Path { get; private set; }

        public string FileSize { get; private set; }

        public string CreateTime { get; private set; }

        public string ChangeTime { get; private set; }

        public string Include { get; private set; }

        private IStorageItem Item;

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyDialog(IStorageItem Item)
        {
            InitializeComponent();
            this.Item = Item;
            Loading += PropertyDialog_Loading;
        }

        public PropertyDialog(IStorageItem Item, string ReplaceName)
        {
            InitializeComponent();
            FileName = ReplaceName;
            this.Item = Item;
            Loading += PropertyDialog_Loading;
        }

        private async void PropertyDialog_Loading(FrameworkElement sender, object args)
        {
            if (Item is StorageFile file)
            {
                IncludeArea.Visibility = Visibility.Collapsed;

                if (string.IsNullOrEmpty(FileName))
                {
                    FileName = file.Name;
                }

                FileType = file.DisplayType + " (" + file.FileType + ")";
                Path = file.Path;
                CreateTime = file.DateCreated.ToString("F");

                if (file.ContentType.StartsWith("video"))
                {
                    VideoProperties Video = await file.Properties.GetVideoPropertiesAsync();
                    ExtraData.Text = $"{Globalization.GetString("FileProperty_Resolution")}: {((Video.Width == 0 && Video.Height == 0) ? "Unknown" : $"{Video.Width}×{Video.Height}")}{Environment.NewLine}{Globalization.GetString("FileProperty_Bitrate")}: {(Video.Bitrate == 0 ? "Unknown" : (Video.Bitrate / 1024f < 1024 ? Math.Round(Video.Bitrate / 1024f, 2).ToString("0.00") + " Kbps" : Math.Round(Video.Bitrate / 1048576f, 2).ToString("0.00") + " Mbps"))}{Environment.NewLine}{Globalization.GetString("FileProperty_Duration")}: {ConvertTimsSpanToString(Video.Duration)}";
                }
                else if (file.ContentType.StartsWith("audio"))
                {
                    MusicProperties Music = await file.Properties.GetMusicPropertiesAsync();
                    ExtraData.Text = $"{Globalization.GetString("FileProperty_Bitrate")}: {(Music.Bitrate == 0 ? "Unknown" : (Music.Bitrate / 1024f < 1024 ? Math.Round(Music.Bitrate / 1024f, 2).ToString("0.00") + " Kbps" : Math.Round(Music.Bitrate / 1048576f, 2).ToString("0.00") + " Mbps"))}{Environment.NewLine}{Globalization.GetString("FileProperty_Duration")}: {ConvertTimsSpanToString(Music.Duration)}";
                }
                else if (file.ContentType.StartsWith("image"))
                {
                    ImageProperties Image = await file.Properties.GetImagePropertiesAsync();
                    ExtraData.Text = $"{Globalization.GetString("FileProperty_Resolution")}: {((Image.Width == 0 && Image.Height == 0) ? "Unknown" : $"{Image.Width}×{Image.Height}")}{Environment.NewLine}{Globalization.GetString("FileProperty_ShootingDate")}: {Image.DateTaken.ToLocalTime():F}{Environment.NewLine}{Globalization.GetString("FileProperty_Longitude")}: {(Image.Longitude.HasValue ? Image.Longitude.Value.ToString() : "Unknown")}{Environment.NewLine}{Globalization.GetString("FileProperty_Latitude")}: {(Image.Latitude.HasValue ? Image.Latitude.Value.ToString() : "Unknown")}";
                }
                else
                {
                    switch (file.FileType)
                    {
                        case ".flv":
                        case ".rmvb":
                        case ".rm":
                        case ".mov":
                        case ".mkv":
                        case ".mp4":
                        case ".m4v":
                        case ".m2ts":
                        case ".wmv":
                        case ".f4v":
                        case ".ts":
                        case ".swf":
                            {
                                VideoProperties Video = await file.Properties.GetVideoPropertiesAsync();
                                ExtraData.Text = $"{Globalization.GetString("FileProperty_Resolution")}: {((Video.Width == 0 && Video.Height == 0) ? "Unknown" : $"{Video.Width}×{Video.Height}")}{Environment.NewLine}{Globalization.GetString("FileProperty_Bitrate")}: {(Video.Bitrate == 0 ? "Unknown" : (Video.Bitrate / 1024f < 1024 ? Math.Round(Video.Bitrate / 1024f, 2).ToString("0.00") + " Kbps" : Math.Round(Video.Bitrate / 1048576f, 2).ToString("0.00") + " Mbps"))}{Environment.NewLine}{Globalization.GetString("FileProperty_Duration")}: {ConvertTimsSpanToString(Video.Duration)}";
                                break;
                            }
                        case ".mpe":
                        case ".flac":
                        case ".mp3":
                        case ".aac":
                        case ".wma":
                        case ".ogg":
                            {
                                MusicProperties Music = await file.Properties.GetMusicPropertiesAsync();
                                ExtraData.Text = $"{Globalization.GetString("FileProperty_Bitrate")}: {(Music.Bitrate == 0 ? "Unknown" : (Music.Bitrate / 1024f < 1024 ? Math.Round(Music.Bitrate / 1024f, 2).ToString("0.00") + " Kbps" : Math.Round(Music.Bitrate / 1048576f, 2).ToString("0.00") + " Mbps"))}{Environment.NewLine}{Globalization.GetString("FileProperty_Duration")}: {ConvertTimsSpanToString(Music.Duration)}";
                                break;
                            }
                        case ".raw":
                        case ".bmp":
                        case ".tiff":
                        case ".gif":
                        case ".jpg":
                        case ".jpeg":
                        case ".exif":
                        case ".png":
                            {
                                ImageProperties Image = await file.Properties.GetImagePropertiesAsync();
                                ExtraData.Text = $"{Globalization.GetString("FileProperty_Resolution")}: {((Image.Width == 0 && Image.Height == 0) ? "Unknown" : $"{Image.Width}×{Image.Height}")}{Environment.NewLine}{Globalization.GetString("FileProperty_ShootingDate")}: {Image.DateTaken.ToLocalTime():F}{Environment.NewLine}{Globalization.GetString("FileProperty_Longitude")}: {(Image.Longitude.HasValue ? Image.Longitude.Value.ToString() : "Unknown")}{Environment.NewLine}{Globalization.GetString("FileProperty_Latitude")}: {(Image.Latitude.HasValue ? Image.Latitude.Value.ToString() : "Unknown")}";
                                break;
                            }
                        default:
                            {
                                ExtraDataArea.Visibility = Visibility.Collapsed;
                                break;
                            }
                    }
                }

                BasicProperties Properties = await file.GetBasicPropertiesAsync();

                FileSize = (Properties.Size / 1024f < 1024 ? Math.Round(Properties.Size / 1024f, 2).ToString("0.00") + " KB" :
                            (Properties.Size / 1048576f < 1024 ? Math.Round(Properties.Size / 1048576f, 2).ToString("0.00") + " MB" :
                            (Properties.Size / 1073741824f < 1024 ? Math.Round(Properties.Size / 1073741824f, 2).ToString("0.00") + " GB" :
                            Math.Round(Properties.Size / Convert.ToDouble(1099511627776), 2).ToString() + " TB"))) + " (" + Properties.Size.ToString("N0") + $" {Globalization.GetString("Device_Capacity_Unit")})";

                ChangeTime = Properties.DateModified.ToString("F");

                OnPropertyChanged();
            }
            else if (Item is StorageFolder folder)
            {
                ExtraDataArea.Visibility = Visibility.Collapsed;

                Include = Globalization.GetString("SizeProperty_Calculating_Text");
                FileSize = Globalization.GetString("SizeProperty_Calculating_Text");

                FileType = folder.DisplayType;

                if (string.IsNullOrEmpty(FileName))
                {
                    FileName = folder.DisplayName;
                }

                Path = folder.Path;
                CreateTime = folder.DateCreated.ToString("F");

                var Properties = await folder.GetBasicPropertiesAsync();
                ChangeTime = Properties.DateModified.ToString("F");

                OnPropertyChanged();

                await Task.Run(() =>
                {
                    CalculateFolderAndFileCount(folder);
                    CalculateFolderSize(folder);
                }).ConfigureAwait(true);

                OnPropertyChanged();
            }
        }

        private string ConvertTimsSpanToString(TimeSpan Span)
        {
            int Hour = 0;
            int Minute = 0;
            int Second = Convert.ToInt32(Span.TotalSeconds);
            if (Second >= 60)
            {
                Minute = Second / 60;
                Second %= 60;
                if (Minute >= 60)
                {
                    Hour = Minute / 60;
                    Minute %= 60;
                }
            }

            return string.Format("{0:D2}:{1:D2}:{2:D2}", Hour, Minute, Second);
        }

        private void CalculateFolderSize(StorageFolder Folder)
        {
            long TotalSize = WIN_Native_API.CalculateSize(Folder.Path);

            FileSize = TotalSize / 1024f < 1024 ? Math.Round(TotalSize / 1024f, 2).ToString("0.00") + " KB" :
            (TotalSize / 1048576f < 1024 ? Math.Round(TotalSize / 1048576f, 2).ToString("0.00") + " MB" :
            (TotalSize / 1073741824f < 1024 ? Math.Round(TotalSize / 1073741824f, 2).ToString("0.00") + " GB" :
            Math.Round(TotalSize / Convert.ToDouble(1099511627776), 2).ToString() + " TB")) + " (" + TotalSize.ToString("N0") + $" {Globalization.GetString("Device_Capacity_Unit")})";
        }

        private void CalculateFolderAndFileCount(StorageFolder Folder)
        {
            (uint FolderCount, uint FileCount) = WIN_Native_API.CalculateFolderAndFileCount(Folder.Path);

            Include = $"{FileCount} {Globalization.GetString("FolderInfo_File_Count")} , {FolderCount} {Globalization.GetString("FolderInfo_Folder_Count")}";
        }

        public void OnPropertyChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FileName)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FileType)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(FileSize)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(CreateTime)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeTime)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Include)));
            }
        }
    }
}
