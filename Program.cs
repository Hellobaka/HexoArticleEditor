using MudBlazor.Services;
using HexoArticleEditor.Components;
using HexoArticleEditor;

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
builder.Services.AddServerSideBlazor().AddHubOptions(opt => { opt.MaximumReceiveMessageSize = AppConfig.MaxFileSize; });
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
