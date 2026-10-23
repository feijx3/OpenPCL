using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;
using NAudio;
using NAudio.Wave;

namespace PCL
{
    [StandardModule]
    public sealed class ModMusic
    {
        private static void MusicListInit(bool ForceReload, string PreventFirst = null)
        {
            if (ForceReload)
            {
                ModMusic.MusicAllList = null;
            }
            try
            {
                if (ModMusic.MusicAllList == null)
                {
                    ModMusic.MusicAllList = new List<string>();
                    DirectoryUtils.Create(Paths.Base + "PCL\\Musics\\");
                    try
                    {
                        foreach (string text in DirectoryUtils.EnumerateFiles(Paths.Base + "PCL\\Musics\\", true, "*"))
                        {
                            if (!new string[] { "ini", "jpg", "txt", "cfg", "lrc", "db", "png" }.Contains(PathUtils.GetExtension(text)))
                            {
                                ModMusic.MusicAllList.Add(text);
                            }
                        }
                    }
                    finally
                    {
                        IEnumerator<string> enumerator;
                        if (enumerator != null)
                        {
                            enumerator.Dispose();
                        }
                    }
                }
                ModMusic.MusicWaitingList = (Settings.Get<bool>("UiMusicRandom", null) ? new List<string>(ModMusic.MusicAllList).Shuffle<string>().ToList<string>() : new List<string>(ModMusic.MusicAllList));
                if (PreventFirst != null && Operators.CompareString(ModMusic.MusicWaitingList.FirstOrDefault<string>(), PreventFirst, false) == 0)
                {
                    ModMusic.MusicWaitingList.RemoveAt(0);
                    ModMusic.MusicWaitingList.Add(PreventFirst);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化音乐列表失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
            }
        }

        private static string DequeueNextMusicAddress()
        {
            if (ModMusic.MusicAllList == null || !ModMusic.MusicAllList.Any<string>() || !ModMusic.MusicWaitingList.Any<string>())
            {
                ModMusic.MusicListInit(false, null);
            }
            string text;
            if (ModMusic.MusicWaitingList.Any<string>())
            {
                text = ModMusic.MusicWaitingList[0];
                ModMusic.MusicWaitingList.RemoveAt(0);
            }
            else
            {
                text = null;
            }
            if (!ModMusic.MusicWaitingList.Any<string>())
            {
                ModMusic.MusicListInit(false, text);
            }
            return text;
        }

        private static void MusicRefreshUI()
        {
            ModBase.RunInUi((ModMusic._Closure$__.$I5-0 == null) ? (ModMusic._Closure$__.$I5-0 = delegate
            {
                try
                {
                    if (!ModMusic.MusicAllList.Any<string>())
                    {
                        ModMain.FrmMain.BtnExtraMusic.Show = false;
                    }
                    else
                    {
                        ModMain.FrmMain.BtnExtraMusic.Show = true;
                        string text;
                        if (ModMusic.MusicState == ModMusic.MusicStates.Pause)
                        {
                            ModMain.FrmMain.BtnExtraMusic.Logo = "M803.904 463.936a55.168 55.168 0 0 1 0 96.128l-463.616 264.448C302.848 845.888 256 819.136 256 776.448V247.616c0-42.752 46.848-69.44 84.288-48.064l463.616 264.384z";
                            ModMain.FrmMain.BtnExtraMusic.LogoScale = 0.8;
                            text = "已暂停：" + PathUtils.GetFileNameWithoutExtension(ModMusic.MusicCurrent);
                            if (ModMusic.MusicAllList.Count > 1)
                            {
                                text += "\r\n左键恢复播放，右键播放下一曲。";
                            }
                            else
                            {
                                text += "\r\n左键恢复播放，右键重新从头播放。";
                            }
                        }
                        else
                        {
                            ModMain.FrmMain.BtnExtraMusic.Logo = "M348.293565 716.53287V254.797913c0-41.672348 28.004174-78.358261 68.919652-90.37913L815.994435 40.826435c62.775652-18.610087 125.907478 26.579478 125.907478 89.933913v539.158261c8.013913 42.25113-8.94887 89.177043-47.014956 127.109565a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c21.014261 1.024 40.514783 5.164522 57.878261 12.065391V233.338435c0-12.109913-10.551652-20.034783-20.569044-20.034783a24.620522 24.620522 0 0 0-5.787826 0.934957L439.785739 338.18713a19.545043 19.545043 0 0 0-14.825739 19.144348v438.984348H423.846957c11.53113 43.987478-5.164522 94.208-45.412174 134.322087a232.848696 232.848696 0 0 1-170.785392 65.758609c-61.885217-2.938435-111.081739-33.435826-129.113043-80.050087-18.031304-46.614261-2.137043-102.177391 41.672348-145.853218a232.848696 232.848696 0 0 1 170.785391-65.80313c20.791652 1.024 40.069565 5.075478 57.299478 11.842783z";
                            ModMain.FrmMain.BtnExtraMusic.LogoScale = 1.0;
                            text = "正在播放：" + PathUtils.GetFileNameWithoutExtension(ModMusic.MusicCurrent);
                            if (ModMusic.MusicAllList.Count > 1)
                            {
                                text += "\r\n左键暂停，右键播放下一曲。";
                            }
                            else
                            {
                                text += "\r\n左键暂停，右键重新从头播放。";
                            }
                        }
                        ModMain.FrmMain.BtnExtraMusic.ToolTip = text;
                    }
                    if (ModMain.FrmSetupUI != null)
                    {
                        ModMain.FrmSetupUI.MusicRefreshUI();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "刷新背景音乐 UI 失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                }
            }) : ModMusic._Closure$__.$I5-0, false);
        }

        public static void MusicControlPause()
        {
            if (ModMusic.MusicNAudio == null)
            {
                ModMain.Hint("音乐播放尚未开始！", ModMain.HintType.Red, true);
                return;
            }
            ModMusic.MusicStates musicState = ModMusic.MusicState;
            if (musicState == ModMusic.MusicStates.Play)
            {
                ModMusic.MusicPause();
                return;
            }
            if (musicState == ModMusic.MusicStates.Pause)
            {
                ModMusic.MusicResume();
                return;
            }
            Logger.Warn("音乐目前为停止状态，已强制尝试开始播放", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
            ModMusic.MusicRefreshPlay(false, false);
        }

        public static void MusicControlNext()
        {
            if (ModMusic.MusicAllList.IsSingle<string>())
            {
                ModMusic.MusicStartPlay(ModMusic.MusicCurrent, false);
                ModMain.Hint("重新播放：" + PathUtils.GetLastPart(ModMusic.MusicCurrent), ModMain.HintType.Green, true);
            }
            else
            {
                string text = ModMusic.DequeueNextMusicAddress();
                if (text == null)
                {
                    ModMain.Hint("没有可以播放的音乐！", ModMain.HintType.Red, true);
                }
                else
                {
                    ModMusic.MusicStartPlay(text, false);
                    ModMain.Hint("正在播放：" + PathUtils.GetLastPart(text), ModMain.HintType.Green, true);
                }
            }
            ModMusic.MusicRefreshUI();
        }

        public static ModMusic.MusicStates MusicState
        {
            get
            {
                ModMusic.MusicStates musicStates;
                if (ModMusic.MusicNAudio == null)
                {
                    musicStates = ModMusic.MusicStates.Stop;
                }
                else
                {
                    object obj = NewLateBinding.LateGet(ModMusic.MusicNAudio, null, "PlaybackState", new object[0], null, null, null);
                    if (Operators.ConditionalCompareObjectEqual(obj, 0, false))
                    {
                        musicStates = ModMusic.MusicStates.Stop;
                    }
                    else if (Operators.ConditionalCompareObjectEqual(obj, 2, false))
                    {
                        musicStates = ModMusic.MusicStates.Pause;
                    }
                    else
                    {
                        musicStates = ModMusic.MusicStates.Play;
                    }
                }
                return musicStates;
            }
        }

        public static void MusicRefreshPlay(bool ShowHint, bool IsFirstLoad = false)
        {
            try
            {
                ModMusic.MusicListInit(true, null);
                if (!ModMusic.MusicAllList.Any<string>())
                {
                    if (ModMusic.MusicNAudio == null)
                    {
                        if (ShowHint)
                        {
                            ModMain.Hint("未检测到可用的背景音乐！", ModMain.HintType.Red, true);
                        }
                    }
                    else
                    {
                        ModMusic.MusicNAudio = null;
                        if (ShowHint)
                        {
                            ModMain.Hint("背景音乐已清除！", ModMain.HintType.Green, true);
                        }
                    }
                }
                else
                {
                    string text = ModMusic.DequeueNextMusicAddress();
                    if (text == null)
                    {
                        if (ShowHint)
                        {
                            ModMain.Hint("没有可以播放的音乐！", ModMain.HintType.Red, true);
                        }
                    }
                    else
                    {
                        try
                        {
                            ModMusic.MusicStartPlay(text, IsFirstLoad);
                            if (ShowHint)
                            {
                                ModMain.Hint("背景音乐已刷新：" + PathUtils.GetLastPart(text), ModMain.HintType.Green, false);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                ModMusic.MusicRefreshUI();
            }
            catch (Exception ex2)
            {
                Logger.Error(ex2, "刷新背景音乐播放失败", LogBehavior.AlertThenFeedback, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
            }
        }

        private static void MusicStartPlay(string Address, bool IsFirstLoad = false)
        {
            if (Address != null)
            {
                Logger.Info(string.Format("播放开始：{0}", Address), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                ModMusic.MusicCurrent = Address;
                ModBase.RunInNewThread(delegate
                {
                    ModMusic.MusicLoop(IsFirstLoad);
                }, "Music", ThreadPriority.BelowNormal);
            }
        }

        public static bool MusicPause()
        {
            bool flag;
            if (ModMusic.MusicState == ModMusic.MusicStates.Play)
            {
                ModBase.RunInThread((ModMusic._Closure$__.$I13-0 == null) ? (ModMusic._Closure$__.$I13-0 = delegate
                {
                    Logger.Info("已暂停播放", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                    object musicNAudio = ModMusic.MusicNAudio;
                    if (musicNAudio != null)
                    {
                        NewLateBinding.LateCall(musicNAudio, null, "Pause", new object[0], null, null, null, true);
                    }
                    ModMusic.MusicRefreshUI();
                }) : ModMusic._Closure$__.$I13-0);
                flag = true;
            }
            else
            {
                Logger.Info(string.Format("无需暂停播放，当前状态为 {0}", ModMusic.MusicState), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                flag = false;
            }
            return flag;
        }

        public static bool MusicResume()
        {
            bool flag;
            if (ModMusic.MusicState == ModMusic.MusicStates.Play || !ModMusic.MusicAllList.Any<string>())
            {
                Logger.Info(string.Format("无需继续播放，当前状态为 {0}", ModMusic.MusicState), LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                flag = false;
            }
            else
            {
                ModBase.RunInThread((ModMusic._Closure$__.$I14-0 == null) ? (ModMusic._Closure$__.$I14-0 = delegate
                {
                    Logger.Info("已恢复播放", LogBehavior.None, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                    try
                    {
                        object musicNAudio = ModMusic.MusicNAudio;
                        if (musicNAudio != null)
                        {
                            NewLateBinding.LateCall(musicNAudio, null, "Play", new object[0], null, null, null, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        object musicNAudio2 = ModMusic.MusicNAudio;
                        if (musicNAudio2 != null)
                        {
                            NewLateBinding.LateCall(musicNAudio2, null, "Stop", new object[0], null, null, null, true);
                        }
                        object musicNAudio3 = ModMusic.MusicNAudio;
                        if (musicNAudio3 != null)
                        {
                            NewLateBinding.LateCall(musicNAudio3, null, "Play", new object[0], null, null, null, true);
                        }
                    }
                    ModMusic.MusicRefreshUI();
                }) : ModMusic._Closure$__.$I14-0);
                flag = true;
            }
            return flag;
        }

        private static void MusicLoop(bool IsFirstLoad = false)
        {
            WaveOutEvent waveOutEvent = null;
            WaveStream waveStream = null;
            try
            {
                waveOutEvent = new WaveOutEvent();
                ModMusic.MusicNAudio = waveOutEvent;
                waveOutEvent.DeviceNumber = -1;
                try
                {
                    waveStream = new AudioFileReader(ModMusic.MusicCurrent);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "使用 AudioFileReader 加载音频文件失败，换用 MediaFoundationReader 重试", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                    waveStream = new MediaFoundationReader(ModMusic.MusicCurrent);
                }
                waveOutEvent.Init(waveStream);
                waveOutEvent.Play();
                if (IsFirstLoad && !Settings.Get<bool>("UiMusicAuto", null))
                {
                    waveOutEvent.Pause();
                }
                ModMusic.MusicRefreshUI();
                int num = 0;
                while (waveOutEvent.Equals(RuntimeHelpers.GetObjectValue(ModMusic.MusicNAudio)) && waveOutEvent.PlaybackState != null)
                {
                    ModMusic._Closure$__17-0 CS$<>8__locals1 = new ModMusic._Closure$__17-0(CS$<>8__locals1);
                    if (Settings.Get<int>("UiMusicVolume", null) != num)
                    {
                        num = Settings.Get<int>("UiMusicVolume", null);
                        waveOutEvent.Volume = (float)((double)num / 1000.0);
                    }
                    CS$<>8__locals1.$VB$Local_Percent = waveStream.CurrentTime.TotalMilliseconds / waveStream.TotalTime.TotalMilliseconds;
                    ModBase.RunInUi(delegate
                    {
                        ModMain.FrmMain.BtnExtraMusic.Progress = CS$<>8__locals1.$VB$Local_Percent;
                    }, false);
                    Thread.Sleep(100);
                }
                if (waveOutEvent.PlaybackState == null && ModMusic.MusicAllList.Any<string>())
                {
                    ModMusic.MusicStartPlay(ModMusic.DequeueNextMusicAddress(), false);
                }
            }
            catch (Exception ex2)
            {
                Logger.Warn(ex2, string.Format("播放音乐出现内部错误（{0}）", ModMusic.MusicCurrent), LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                if (ex2 is MmException && ex2.Message.Contains("AlreadyAllocated"))
                {
                    ModMain.Hint("你的音频设备正被其他程序占用。请在关闭占用的程序后重启 PCL，才能恢复音乐播放功能！", ModMain.HintType.Red, true);
                    Thread.Sleep(1000000000);
                }
                if (ex2 is MmException && (ex2.Message.Contains("NoDriver") || ex2.Message.Contains("BadDeviceId")))
                {
                    ModMain.Hint("由于音频设备变更，音乐播放功能在重启 PCL 后才能恢复！", ModMain.HintType.Red, true);
                    Thread.Sleep(1000000000);
                }
                if (!new string[] { "wav", "mp3", "flac" }.Contains(PathUtils.GetExtension(ModMusic.MusicCurrent)) || ex2.Message.Contains("0xC00D36C4"))
                {
                    ModMain.Hint("播放音乐失败（" + PathUtils.GetLastPart(ModMusic.MusicCurrent) + "）：PCL 可能不支持此音乐格式，请将格式转换为 .wav、.mp3 或 .flac 后再试", ModMain.HintType.Red, true);
                }
                else
                {
                    Logger.Error(ex2, string.Format("播放音乐失败（{0}）", PathUtils.GetLastPart(ModMusic.MusicCurrent)), LogBehavior.Toast, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\ModMusic.vb");
                }
                ModMusic.MusicAllList.Remove(ModMusic.MusicCurrent);
                ModMusic.MusicWaitingList.Remove(ModMusic.MusicCurrent);
                ModMusic.MusicRefreshUI();
                Thread.Sleep(2000);
                if (ex2 is FileNotFoundException)
                {
                    ModMusic.MusicRefreshPlay(true, IsFirstLoad);
                }
                else
                {
                    ModMusic.MusicStartPlay(ModMusic.DequeueNextMusicAddress(), IsFirstLoad);
                }
            }
            finally
            {
                if (waveOutEvent != null)
                {
                    waveOutEvent.Dispose();
                }
                if (waveStream != null)
                {
                    waveStream.Dispose();
                }
                ModMusic.MusicRefreshUI();
            }
        }

        public static List<string> MusicWaitingList = null;

        public static List<string> MusicAllList = null;

        public static object MusicNAudio = null;

        private static string MusicCurrent = "";

        public enum MusicStates
        {
            Stop,
            Play,
            Pause
        }
    }
}
