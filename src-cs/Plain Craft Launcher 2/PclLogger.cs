using System;
using System.Threading;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class PclLogger : FileLogger
    {
        public override string Format(string Text, LogLevel Level, string FilePath, Exception Ex)
        {
            Text = base.Format(Text, Level, FilePath, Ex);
            Text = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(Text, '*'), '*');
            return Text;
        }

        public override void HandleBehavior(string RawMessage, string FormattedMessage, LogBehavior Behavior, Exception Ex)
        {
            if (!ModBase.IsProgramEnding)
            {
                base.HandleBehavior(RawMessage, FormattedMessage, Behavior, Ex);
                string text = ((Ex == null) ? RawMessage : (((RawMessage == null) ? "" : string.Format("{0}：", RawMessage)) + Ex.GetDisplay(false)));
                text = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(text, '*'), '*');
                string text2 = ((Ex == null) ? RawMessage : (((RawMessage == null) ? "" : string.Format("{0}：", RawMessage)) + Ex.GetDisplay(true)));
                text2 = ModMinecraft.FilterUserName(ModMinecraft.FilterAccessToken(text2, '*'), '*');
                switch (Behavior)
                {
                case LogBehavior.ToastIfDebug:
                    if (ModBase.ModeDebug)
                    {
                        ModMain.Hint("[调试模式] " + text, ModMain.HintType.Blue, false);
                    }
                    break;
                case LogBehavior.Toast:
                    ModMain.Hint(text, ModMain.HintType.Red, false);
                    break;
                case LogBehavior.Alert:
                    ModMain.MyMsgBox(text2, "错误", "确定", "", "", true, true, false, null, null, null);
                    break;
                case LogBehavior.AlertThenFeedback:
                    if (PageSetupSystem.IsLauncherNewest().GetValueOrDefault())
                    {
                        if (ModMain.MyMsgBox(text2 + "\r\n\r\n是否反馈此问题？如果不反馈，这个问题可能永远无法得到解决！", "错误", "反馈", "取消", "", true, true, false, null, null, null) == 1)
                        {
                            ModMain.Feedback(false, true);
                        }
                    }
                    else
                    {
                        ModMain.MyMsgBox(text2 + "\r\n\r\n将 PCL 更新至最新版或许可以解决这个问题……", "错误", "确定", "", "", true, true, false, null, null, null);
                    }
                    break;
                case LogBehavior.AlertThenCrash:
                {
                    if (this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init == null)
                    {
                        Interlocked.CompareExchange<StaticLocalInitFlag>(ref this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init, new StaticLocalInitFlag(), null);
                    }
                    bool flag = false;
                    try
                    {
                        Monitor.Enter(this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init, ref flag);
                        if (this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State == 0)
                        {
                            this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State = 2;
                            this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger = true;
                        }
                        else if (this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State == 2)
                        {
                            throw new IncompleteInitialization();
                        }
                    }
                    finally
                    {
                        this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init.State = 1;
                        if (flag)
                        {
                            Monitor.Exit(this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init);
                        }
                    }
                    if (this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger)
                    {
                        this.$STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger = false;
                        if (PageSetupSystem.IsLauncherNewest().GetValueOrDefault())
                        {
                            if (Interaction.MsgBox(text2 + "\r\n\r\n是否反馈此问题？如果不反馈，这个问题可能永远无法得到解决！", MsgBoxStyle.YesNo | MsgBoxStyle.Critical, "错误") == MsgBoxResult.Yes)
                            {
                                ModMain.Feedback(false, true);
                            }
                        }
                        else
                        {
                            Interaction.MsgBox(text2 + "\r\n\r\n将 PCL 更新至最新版或许可以解决这个问题……", MsgBoxStyle.Critical, "错误");
                        }
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }
                    FormMain.EndProgramForce(ModBase.ProcessReturnValues.Exception);
                    break;
                }
                }
                if (Behavior >= LogBehavior.Toast)
                {
                    ModMain.Telemetry("错误日志", new string[] { "Exception", text2 });
                }
            }
        }

        private bool $STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger;

        private StaticLocalInitFlag $STATIC$HandleBehavior$2041EE1181E91280CD$FirstTrigger$Init;
    }
}
