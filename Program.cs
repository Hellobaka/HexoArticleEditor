using MudBlazor.Services;
using HexoArticleEditor.Components;
using HexoArticleEditor;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components.Authorization;
using HexoArticleEditor.Auth;

AppConfig.Load();
if (string.IsNullOrEmpty(AppConfig.HexoBasePath))
{
    Console.Write("似乎是第一次启动软件呢！需要指定一下 Hexo 的目录：");
    string path = Console.ReadLine();
    if (string.IsNullOrEmpty(path))
    {
        Console.WriteLine("输入的路径无效，找到目录了再来吧");
        return;
    }
    if (Directory.Exists(path))
    {
        if(!File.Exists(Path.Combine(path, "_config.yml")))
        {
            Console.WriteLine("未在此目录下找到 _config.yml. 可能提供的并不是 Hexo 根目录");
            return;
        }
    }
    else
    {
        Console.WriteLine("未找到此目录，找到目录了再来吧");
        return;
    }

    Console.WriteLine($"文章路径：{Path.Combine(path, "source\\_posts")}");
    Console.WriteLine($"存储图片路径：{Path.Combine(path, "public\\images\\post")}");
    Console.WriteLine("以上路径是否正确？[y/n]");
    if (Console.ReadLine().Trim().ToLower() == "y")
    {
        ConfigHelper.SetConfig("HexoBasePath", path);
        ConfigHelper.SetConfig("HexoArticlePath", Path.Combine(path, "source\\_posts"));
        ConfigHelper.SetConfig("HexoImagePath", Path.Combine(path, "public\\images\\post"));

        AppConfig.HexoBasePath = path;
        AppConfig.HexoArticlePath = Path.Combine(path, "source\\_posts");
        AppConfig.HexoImagePath = Path.Combine(path, "public\\images\\post");
    }
    else
    {
        Console.Write("Hexo 的文章路径：");
        path = Console.ReadLine();
        ConfigHelper.SetConfig("HexoArticlePath", path);
        AppConfig.HexoArticlePath = path;

        Console.Write("Hexo 的存储图片路径：");
        path = Console.ReadLine();
        ConfigHelper.SetConfig("HexoImagePath", path);
        AppConfig.HexoImagePath = path;
    }

    Console.Write("保证 Shell 安全我们需要进行简单的鉴权，请输入一个复杂一些的密码：");
    string password = Console.ReadLine();
    ConfigHelper.SetConfig("Password", password);
    AppConfig.Password = password;
}

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddSignalR(e => { e.MaximumReceiveMessageSize = AppConfig.MaxFileSize; }); 
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<SessionStorageProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<SessionStorageProvider>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
CreateSymbolLinkToImageFolder();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

void CreateSymbolLinkToImageFolder()
{
    string path1 = Path.Combine(AppConfig.HexoBasePath, "public", "images");
    string path2 = Path.GetFullPath("wwwroot") + "\\images";
    bool result;
    if (Directory.Exists(path2))
    {
        Directory.Delete(path2, true);
    }
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        result = CreateSymbolicLinkWindows(path2, path1);
        if (!result)
        {
            Console.WriteLine("[-]GetLastError = " + Marshal.GetLastWin32Error());
        }
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        result = CreateSymbolicLinkLinux(path2, path1);
    }
    else
    {
        throw new PlatformNotSupportedException();
    }

    if (!result)
    {
        Console.WriteLine("[-]创建符号链接被拒绝，请提升权限");
    }
}

[DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode, SetLastError = true)] 
static extern int CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, uint dwFlags);

const uint SYMBOLIC_LINK_FLAG_FILE = 0x0;
const uint SYMBOLIC_LINK_FLAG_DIRECTORY = 0x1;

bool CreateSymbolicLinkWindows(string symlink, string target)
{
    uint flags = SYMBOLIC_LINK_FLAG_FILE;
    symlink = Path.GetFullPath(symlink);

    // Check if the target is a directory
    if (Directory.Exists(target))
    {
        flags = SYMBOLIC_LINK_FLAG_DIRECTORY;
    }

    return CreateSymbolicLink(symlink, target, flags) == 1;
}

bool CreateSymbolicLinkLinux(string symlink, string target)
{
    try
    {
        Process process = new Process();
        process.StartInfo.FileName = "ln";
        process.StartInfo.Arguments = $"-s \"{target}\" \"{symlink}\"";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        process.WaitForExit();

        return process.ExitCode == 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        return false;
    }
}