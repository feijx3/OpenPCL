# 🥔 OpenPCL 2.99.0.0

### 《一个启动器的自我修养》

![版本](https://img.shields.io/badge/内部版本码-414-blue)
![来源](https://img.shields.io/badge/来源-dnSpy-orange)
![反编译耗时](https://img.shields.io/badge/反编译耗时-7%20秒-red)
![遥测](https://img.shields.io/badge/遥测-默认开启-yellow)
![goto](https://img.shields.io/badge/goto%20标签-307-lightgrey)

> 想看这个启动器的源码？不用求，**七秒钟**就好。
> 本仓库不是原作者写的，是反编译器写的，笑点解析也不是反编译器写的，是 Claude Sonnet 5.5 写的。
> 下面每一条都能在源码里找到出处（文件 + 函数名）
> 声明：我们只静态阅读了代码，没有运行过；"客户端**能**做某件事"不等于"有人**做过**这件事"，请勿过度联想——我们只是觉得有些词，比如"公告"，被委屈了。

---

## 猎奇清单

### 1. 更新是一项会员权益

`ModSecret.cs` → `HasUpdatePermission`、`UpdateCheckByButton`

在这个构建里，点"检查更新"发现有新版本后，如果你**没有更新权限**（赞助档位 ≥ 12，或土豆码里带更新特性），弹出来的不是"立即更新"，而是《验证爱发电赞助》。有权限的人才配看到那个写着"更新！冲！"的按钮；没权限的，请先冲，再更新。

软件更新，从"安全补丁"进化成了"会员专属"。

### 2. 土豆码：一张绑定设备的赞助小票

`ModSecret.cs` → `CheckPotatoCode`、`GeneratePotatoCode`

- 土豆码内含 ECDSA 签名，签的内容包括**你的设备识别码**，换台电脑基本就失效。
- 校验失败的报错分了**至少九种**：你把识别码当土豆码填了、把解锁码当土豆码填了、把更新密钥当土豆码填了、这根本不是土豆码、要新版才能用、已经过期（形容为不新鲜）、大小写输错……每种都有专属文案。这份用心，全花在了教你怎么正确地出示付款凭证上。
- 生成土豆码的函数 `GeneratePotatoCode()` 在客户端里是**空的**。土豆工厂在服务器上，客户端只负责验货——和验钞机一个思路。

### 3. 颜色按价出售

`ModSecret.cs` → `ThemeCheckOne`、`ThemeUnlock`；`PageSetupUI.xaml.cs`；`FormMain.xaml.cs` → `RunCountSub`

- 编号 0–4 和 42 的主题免费，其余需要解锁。换个界面颜色，也要走流程。
- **秋仪金**需要累积赞助 ¥23.33。代码里专门给它写了一句 `throw`，堵死了所有"普通方式"的解锁路径。想
- 有一个主题要求你先解锁其他 9 个主题里的至少 5 个，才可解锁。**集邮**，是这个启动器的核心玩法之一。
- 输入 `PuzzleUnlock` 加设备识别码的末几位，解锁极客蓝。谜面写在界面里，谜底写在源码里，**反编译器 7 秒通关**。

### 4. 设备识别码：用字符串拼接防 grep

`ModSecret.cs` → `GetIdentify`

读取注册表 `HKLM\SYSTEM\HardwareConfig` 下的 `LastConfig`，再混入一个本地生成并保存下来的串（由时间戳和可用内存拼成），哈希之后拼成 `XXXX-XXXX-XXXX-XXXX` 的"识别码"。

作者把路径里的 `Hardware` 拆成了 `Har{0}wareConfig`，再用 `"D".Lower()` 填进去。想必是为了防 grep。2026 年了，仍然相信 grep 是最强的逆向工具，这份朴素令人动容。

拿不到识别码时返回 `0000-0000-0000-0000`，继续往下跑。

### 5. "公告"的权限，比公告大

`ModSecret.cs` → `ServerSub`、`ServerRequirement`

客户端会从腾讯云 COS 拉取 `notice.cfg`、`ServerConfig.json` 和 `notice.json`。一条"公告"里可以包含：

| 类别 | 客户端能识别的内容 |
|---|---|
| **条件**（决定你能不能看到） | 指定设备识别码；启动次数；版本码；某个设置项的值；某个主题是否已解锁；以及 `d10000 <=`——**投一个一万面的骰子决定你要不要看到** |
| **动作**（点了按钮之后执行） | 往你的剪贴板写内容；打开网页；**关闭启动器**；**直接改写设置项**；触发更新或"静默更新"（协议里拼作 `slientupdate`，拼写错误也一并出厂） |

这个"公告栏"其实是一个带灰度发布和定向推送的**小型远程控制台**。一条公告，能关掉你的启动器、改你的设置、往你剪贴板里塞东西，还能先掷个骰子看看要不要通知你。叫它"公告"，是对"控制台"三个字的谦虚。

再说一遍：这只是**客户端具备的能力**，源码里看不出服务器端发没发过。另外，"静默更新"这条路，同样要求更新权限。

### 6. 遥测：默认开启

`ModMain.cs` → `Telemetry`；`Settings.cs`；`PageSetupSystem.xaml`

设置项 `SystemSystemTelemetry` 的默认值是 `true`。上报的事件包括：启动、Minecraft 启动成功 / 失败 / 崩溃、联机开始 / 成功 / 失败、文件下载失败（带文件名和异常文本）、错误日志（带异常文本），发往腾讯云 CLS。日志里管它叫"匿名数据上报"。

想关？设置页里有开关，默认是开的，请自行寻找、自行取消勾选。

### 7. 版本检查 = 比大小

`PageSetupSystem.xaml.cs` → `IsLauncherNewest`；`ModSecret.cs` → `SecretHeadersSign`

启动器对外叫 2.99，对内叫 **414**。服务器给的数字小于等于 414，就算"已经是最新"。版本管理，朴素得让人想起小学算术。

414 已经渗透进请求头：`Referer` 填的是 `http://414.pcl2.server/`，而 `.server` 并不是一个真实存在的顶级域名。是对数字的执念，还是对 RFC 的不屑，本文不作评价。

### 8. CurseForge 密钥随客户端分发

`ModSecret.cs` → `SecretHeadersSign`

在这个构建里，每一份 exe 里都躺着**同一把** CurseForge API 密钥，外面只用 base64 轻轻盖了一层。base64 是编码，不是加密——这是常识，只是常识不收费，所以没人提醒。

本仓库已经替作者回收了。不用谢。

### 9. CDN 防盗链：盐就放在客户端里

`ModSecret.cs` → `SecretCdnSign`

下载地址带 `{CDN}` 标记时，客户端用时间戳、随机数和一个固定的盐（`PCL2Server`）做 MD5 签名，附到链接后面。盐在客户端里，所以它防的是"君子"。把锁的钥匙挂在门把手上，再贴一张"请勿使用"，大概就是这个意思。

### 10. "设置导入 / 导出"：按钮在，功能没有

`PageSetupSystem.xaml.cs` → `BtnSystemSettingExp_Click`、`BtnSystemSettingImp_Click`

两个按钮都在，点下去都只会弹一句"该功能尚在开发中"。把收费入口做得这么完整的同时，设置导出还没做完——这个优先级，无可指摘。

### 11. `My` 开头的类型：47 个（现已全部改名 `Our`）

原来是 `MyButton`、`MyCard`、`MyCheckBox`、`MyMsgText`……全程 "My"，一个 "Our" 都没有。命名上的团结程度，令人羡慕。

**OpenPCL 已经把它们全改成了 `OurButton`、`OurCard`、`OurCheckBox`、`OurMsgText`……**，这一条不再成立。（VB 编译器自己认的名字，如 `MyBase`、`MyClass`、`MyProject`，和一条叫 `MyDownload` 的磁盘路径没动，见下文《OpenPCL：重命名与构建》。）

### 12. 作者的 E 盘

日志调用把源文件路径一起编进了 exe，所以我们知道作者的 E 盘里有个 `Projects\PCL\本体代码` 文件夹。这大概是这个 exe 里，**公开得最彻底的一条信息**。

---

## 综上

这是一个把"更新""主题""公告""遥测"都做成了运营手段的启动器。它确实也能启动 Minecraft，据说。

## 数据

| 项目 | 数值 |
|---|---|
| 源文件 | 349 个 × C# / VB 两种语言 |
| C# 总行数 | ≈ 93,786 |
| 最长的文件 | `ModDownloadLib.cs`，3,421 行 |
| 静态 `Module` | 20 个 |
| `My` 开头的类型 | 47 个（OpenPCL 中已改为 `Our`） |
| `VB$AnonymousDelegate_N.cs` | 27 个（编译器生成，没人认领） |
| `goto` 标签（`IL_xxxx:`） | 307 个（VB 的 `On Error` 翻译成 C# 的遗迹） |
| 弹出"提示气泡"的调用 | 232 处 |

## 反编译工时（真实数据）

| 项目 | 数值 |
|---|---|
| 反编译器 | dnSpy（命令行版） |
| 反编译本体耗时 | **各约 7 秒**（C# 与 VB 各跑一次，每次含主程序和 3 个内嵌库） |
| 作案者 | **Claude Sonnet 5.5**（`claude-sonnet-5-5`） |
| 全程耗时 | **约 20 分钟**（找工具、提取内嵌 DLL、清理密钥、读源码、核实笑点、写本 README） |
| Token | 读取时上下文占用约 **24.5 万**（窗口 100 万）。这是上下文大小，不是累计账单——每一轮都要重读上下文，累计消耗更高，具体数字本人读不到，不瞎编 |

反编译 7 秒，读懂它花了 20 分钟。**拆开一个东西，永远比看懂它快得多。**

## OpenPCL：重命名与构建

> 这一节说的是 **OpenPCL** 这个分支做了什么，以及怎么把它重新编出一个 `OpenPCL.exe`。

### 改了什么

- **`My*` → `Our*`**：所有 `My` + 大写字母开头的类型、控件、方法、文件、XAML 样式键、`PCL.My` 命名空间和反编译器起的局部变量（`myRect` → `ourRect`）。
- **软件改名为 OpenPCL**：程序集名、工程 / 图标 / 解决方案名、窗口标题、`pack://` 资源地址、运行时拼出来的 XAML 里的 `assembly=`。产物是 `OpenPCL.exe`。标题栏左上角那个用矢量线条画的 "PCL" 徽标也重画成了 "OpenPCL"（原来 PCL 三个字母的路径原样保留，前面补了 Open，窗口里的彩蛋徽标同步）。关于页和 exe 图标里的六边形 "PCL" 是位图，没改。
- **刻意没动**：命名空间 `PCL`；`PCL\` 数据目录与注册表键；`PCL2 …` 开头的日志文件名 / 皮肤包名 / User-Agent / CDN 盐（改了会让现有数据和服务端校验失效）；作者源码路径字符串；`MyBase`、`MyClass` 等 VB 关键字；版权声明。
- **遥测、公告、更新这些行为一个字没改**——它们和上面的猎奇清单里写的一样，仍在原处。想关就去设置页，或者另开一个 PR。

### 怎么构建

需要 Windows、.NET Framework 4.8，以及带 Roslyn 的 **Visual Studio 2019+ / Build Tools 的 MSBuild**（装上"使用 .NET 的桌面开发"工作负载，WPF 编译目标才在）。.NET Framework 自带的老 MSBuild 只会 C# 5，编不了。

```powershell
.\build.ps1                      # 自动找 MSBuild（vswhere / 常见路径）
.\build.ps1 -MSBuild "D:\VSS\MSBuild\Current\Bin\MSBuild.exe"
```

产物：`dist\OpenPCL.exe`。脚本会先把内嵌在 `PCL.Resources.resx` 里的第三方程序集解出来放进 `src-cs\lib`（仅作编译引用；运行时程序仍然从自己的资源里加载它们，和原版一样是单文件）。

三个库（`MeloongCore`、`MeloongCore.Wpf`、`PCLCS`）**不是从源码编的**：它们是 C# 12 写的，反编译出来要大改；而且里面没有 `My*`。构建直接用原 exe 里提取出来的那三个 DLL，所以 `src-cs/MeloongCore*`、`src-cs/PCLCS` 只是供阅读的反编译源码。

### 为了让反编译产物真的能编、能跑，做了哪些事

默认选项反编译出来的 C# **根本编不过**（约 4,400 处非法标识符、30 处 VB `On Error` 反编译失败、编译器生成的 139 个闭包类被藏起来了……）。所以 `src-cs/OpenPCL` 不是原来那份反编译，而是用 `dnSpy.Console --show-all --no-anon-methods …` 对 exe **重新反编译**后再修的：

- 合法化 `$STATIC$…`、`CS$<>8__locals1`、`<>c__DisplayClass` 之类的名字；
- 15 个 `On Error Resume Next` 方法按"出错后从下一条顶层语句继续"重写；
- VB 的 `WithEvents` 属性与 XAML 生成的字段重名：属性删掉，或改成 `X_WithEvents` 并在 `InitializeComponent()` 之后接线（保留原来的挂 / 摘事件代码）；
- VB 迭代器状态机、`Implements`、`ByRef` 默认值、`For Each` 迭代变量赋值、`If(a, b)`、委托宽松转换等 VB 惯用法的 C# 等价写法；
- 补回 `Application.xaml` 里反编译丢掉的 `StartupUri="FormMain.xaml"`（原版把它写在生成的 `InitializeComponent` 里，BAML 反编译看不到——不补的话只有启动画面，没有主窗口）；
- 内嵌资源的名字固定成 `PCL.Resources.resources`（VB 的命名规则），否则运行时加载不到内嵌 DLL；
- `Prefer32Bit=false`：原版是 AnyCPU（64 位进程），要提取的 `libwebp64.dll` 也是 64 位的。

### 验证了什么（以及没验证什么）

- 反编译器会悄悄丢掉 `op_Implicit` 调用和把 VB 的 `Is` 写成 `==`：这类错误**能编译、不报错、行为不对**。所以拿原 exe 和新 exe 逐方法对比了 IL 里转换运算符和 `==` / `!=` 运算符的调用数，补回了 `MyColor→Color / SolidColorBrush` 的 13 处转换，并用 Roslyn 把 105 处本应是引用比较的 `==` / `!=`（`McInstance`、`NetThread`、`ResourceProject`、`SearchRequest`）改成了引用比较——其中 `McInstance` 的 `==` 实现里那一处会无限递归，直接栈溢出。
- 在隔离目录（`%APPDATA%` 重定向、网络指向一个拒绝连接的代理）里启动过：进入主窗口、界面与主题正常渲染、日志的启动序列与原版一致。**没有**实际启动过游戏、登录、下载、联机，也没有逐页点过 UI。
- 这是反编译再编译的产物，**不保证与原版逐字节、逐行为一致**。遇到问题欢迎对照原 exe 排查。

## 仓库说明

```text
.
├── build.ps1        # 构建 OpenPCL（见上）
├── src-cs/
│   ├── OpenPCL/            # 可构建的 C# 主程序（重命名、修复后的版本）
│   ├── MeloongCore*/ PCLCS/  # 三个库的反编译源码，仅供阅读，不参与构建
│   ├── Directory.Build.props # 构建配置（语言版本、框架引用路径、64 位）
│   └── solution.sln
└── src-vb/          # VB 版（更接近原貌，goto 也更多；同样改了名，但没有构建过）
```

- 只包含 `.cs` / `.vb` / `.xaml` 和工程文件。**没有** exe、dll、图片、字体、资源 blob，**没有**任何人的 API 密钥。构建需要的那些（图片、字体、`PCL.Resources.resx`）在本地工作区里，但被 `.gitignore` 挡在仓库外——从干净的克隆构建前，要先用 dnSpy 对原 exe 重新导出一遍。
- `PCLCS`、`MeloongCore`、`MeloongCore.Wpf` 三个库原本以字节数组藏在 exe 的资源里，已被提取并一并反编译。
- 变量名里的 `flag`、`text2`、`CS_locals1` 是反编译器起的，不是作者的审美。

## 免责声明

- 本仓库与 Plain Craft Launcher 的原作者**无任何关联**。
- 源码由反编译器自动生成，**版权归全体我的世界用户所有**。本文属于对程序行为的批判性性评论，仅供学习与研究。
- 上文《猎奇清单》里的所有描述均来自对代码的静态阅读，**未经运行验证**。如有错误，欢迎指出，我们会改。（OpenPCL 构建物的验证范围见《OpenPCL：重命名与构建》。）
- 如果原权利人对本仓库有异议，请联系仓库维护者。虽然我也懒得鸟你^^
