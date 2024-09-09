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
    Console.Write("�ƺ��ǵ�һ����������أ���Ҫָ��һ�� Hexo ��Ŀ¼��");
    string path = Console.ReadLine();
    if (string.IsNullOrEmpty(path))
    {
        Console.WriteLine("�����·����Ч���ҵ�Ŀ¼��������");
        return;
    }
    if (Directory.Exists(path))
    {
        if(!File.Exists(Path.Combine(path, "_config.yml")))
        {
            Console.WriteLine("δ�ڴ�Ŀ¼���ҵ� _config.yml. �����ṩ�Ĳ����� Hexo ��Ŀ¼");
            return;
        }
    }
    else
    {
        Console.WriteLine("δ�ҵ���Ŀ¼���ҵ�Ŀ¼��������");
        return;
    }

    Console.WriteLine($"����·����{Path.Combine(path, "source\\_posts")}");
    Console.WriteLine($"�洢ͼƬ·����{Path.Combine(path, "public\\images\\post")}");
    Console.WriteLine("����·���Ƿ���ȷ��[y/n]");
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
        Console.Write("Hexo ������·����");
        path = Console.ReadLine();
        ConfigHelper.SetConfig("HexoArticlePath", path);
        AppConfig.HexoArticlePath = path;

        Console.Write("Hexo �Ĵ洢ͼƬ·����");
        path = Console.ReadLine();
        ConfigHelper.SetConfig("HexoImagePath", path);
        AppConfig.HexoImagePath = path;
    }

    Console.Write("��֤ Shell ��ȫ������Ҫ���м򵥵ļ�Ȩ��������һ������һЩ�����룺");
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
        Console.WriteLine("[-]�����������ӱ��ܾ���������Ȩ��");
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