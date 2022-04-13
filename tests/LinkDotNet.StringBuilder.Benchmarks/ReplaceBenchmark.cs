using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class ReplaceBenchmark
{
    private const string Text =
        @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed vitae urna non leo dictum vestibulum eu quis massa. Aliquam pellentesque tempus porttitor. Nulla id enim id quam rhoncus condimentum. Nullam laoreet ornare pellentesque. Curabitur porta metus eget arcu aliquam tempor. Integer cursus enim ac efficitur finibus. Aenean sollicitudin ante leo, in facilisis tellus ultrices id. Morbi mi lacus, dictum non volutpat interdum, auctor vel dui.

Duis consectetur ac nunc ac auctor. Curabitur eget quam sit amet neque porttitor mattis vitae quis nulla. Etiam eleifend venenatis sapien, id ultrices neque iaculis eget. Curabitur cursus libero sodales, commodo purus hendrerit, elementum elit. Curabitur a nibh nec eros suscipit consectetur. Fusce nec leo dictum, sagittis erat in, sollicitudin ipsum. Proin mattis feugiat facilisis. Sed et maximus justo. Maecenas eget varius metus. Maecenas eleifend placerat placerat. Maecenas eget vestibulum quam. Sed id urna ultricies, pellentesque diam et, ultricies velit. Donec fermentum at nunc sed pellentesque.

Proin vulputate maximus nisl, quis dignissim velit rutrum pretium. Maecenas sed pharetra leo, eu semper ante. Sed erat dui, viverra quis commodo ut, viverra at tellus. Nullam pharetra, dolor eget varius consectetur, lacus nunc fermentum metus, at pretium enim est id justo. Mauris sed tincidunt felis. Curabitur eget vestibulum dolor, quis varius risus. Morbi erat metus, molestie non risus auctor, tempus vulputate purus. Ut ipsum tellus, posuere sed felis sed, facilisis efficitur justo. Aenean placerat molestie ex in ullamcorper. Mauris ex purus, vulputate ac nibh ut, bibendum mattis tortor. Nulla risus tellus, finibus sed fermentum id, hendrerit in urna. Integer sit amet efficitur sapien. Mauris posuere condimentum ipsum, quis ultricies ex tristique eget.

In eleifend tellus quis tincidunt commodo. Suspendisse potenti. Vestibulum dapibus congue imperdiet. Suspendisse et felis ac mi volutpat dignissim. Suspendisse feugiat tincidunt ipsum nec finibus. Nulla efficitur arcu pretium elit mattis ornare. Donec scelerisque dolor lacus, et mollis mauris vulputate nec. Suspendisse elit leo, efficitur eu justo sed, luctus imperdiet lorem. Donec pellentesque, massa semper posuere commodo, sapien magna rhoncus felis, et ultrices quam tellus ut purus. Curabitur eu fermentum nisl. Morbi malesuada pulvinar est, nec cursus massa aliquet a.

Cras suscipit blandit massa, non efficitur justo mollis sit amet. Fusce tellus mauris, maximus quis urna in, sollicitudin dictum est. Vivamus consectetur lorem quis turpis finibus aliquet. Nulla leo odio, lobortis viverra convallis sed, fringilla eu ligula. Nunc vitae metus ex. Suspendisse molestie orci ut nunc aliquet viverra. Donec id rutrum mi. Sed quis laoreet mi, vel mollis risus. Aliquam eget justo mattis, mollis urna ut, maximus sapien. Pellentesque sit amet fringilla quam, et fringilla dui.

Nam non blandit diam. Sed nec erat sollicitudin, fringilla leo ac, gravida ante. Sed molestie rutrum nulla, nec ultricies nulla laoreet ac. Vestibulum quis magna non turpis rhoncus viverra vel eget lacus. Aliquam quis est ultricies, hendrerit erat id, accumsan tortor. Phasellus iaculis vitae massa eu volutpat. Pellentesque lectus mi, pellentesque sit amet pretium aliquam, facilisis vitae mi.

Duis tempor lacus nulla, in consequat velit bibendum vel. Fusce a libero lacinia nunc commodo consectetur eget vel magna. Vestibulum ante elit, lacinia a laoreet et, malesuada non erat. Vivamus at ante non orci lacinia tempus id vel ante. Integer magna nulla, egestas vel ex ut, posuere porta orci. Donec vitae neque augue. Maecenas efficitur pharetra felis sit amet consequat. Donec placerat felis leo, eget dapibus libero vehicula eu. Pellentesque in fermentum orci.

Nullam turpis metus, efficitur id efficitur vel, fermentum at velit. Sed a eleifend nunc. Etiam tempor suscipit nibh, vel efficitur diam semper ac. Aliquam euismod fringilla justo consequat convallis. Proin nibh diam, egestas vitae tortor id, venenatis scelerisque augue. Sed hendrerit dolor elementum, tempor tortor quis, aliquet risus. Praesent feugiat quam id erat laoreet efficitur quis eget velit. Praesent a rutrum nulla. Mauris vel dignissim purus. Phasellus ornare purus nunc, at vehicula velit tempor eget. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Proin pretium pretium fermentum. Suspendisse at pharetra ipsum, in malesuada turpis. Ut id tincidunt justo. Ut dignissim velit ac sollicitudin porta.

Pellentesque feugiat congue libero, eget fringilla mi ornare quis. Proin ante sem, imperdiet quis venenatis et, pellentesque sed turpis. Mauris nibh lectus, elementum sed bibendum id, pretium et libero. Mauris gravida tempus lacus eu consectetur. Ut pulvinar elit purus, ut consequat arcu feugiat id. Suspendisse sed pretium leo, et aliquet metus. Aliquam erat volutpat. Pellentesque iaculis diam lacus, quis dictum mauris feugiat sed. Morbi vitae est a mi elementum facilisis a nec lectus. Integer id fermentum nulla. Donec tempor magna quis enim dignissim maximus. Aenean volutpat tincidunt faucibus.

Morbi maximus venenatis enim, in accumsan turpis aliquet nec. Ut tellus magna, interdum sed arcu sit amet, aliquam scelerisque risus. Phasellus a nunc mollis, gravida arcu vitae, tincidunt felis. Mauris sollicitudin, erat ac pretium ullamcorper, nibh odio vehicula sem, sit amet lobortis lacus orci sed nulla. Aenean sed ligula ac velit accumsan eleifend nec ac nunc. In diam ex, sodales sit amet convallis viverra, elementum sit amet arcu. Proin laoreet mauris vel eleifend dignissim. Curabitur non velit eget ex dictum porta in sed est. Nulla tempus magna vitae convallis cursus.

Fusce vestibulum neque arcu. In vitae felis felis. Quisque sed dictum eros. Fusce commodo nibh velit, sed accumsan tortor gravida vel. Mauris tincidunt fringilla arcu nec ultricies. Nam non efficitur velit. Quisque ac maximus risus. Etiam vulputate tortor ac felis fermentum, in malesuada elit porttitor.

Aliquam erat volutpat. Sed mollis eu nibh id feugiat. Cras a vestibulum arcu, eget aliquam orci. Etiam ut lacinia massa. Praesent non augue fringilla neque scelerisque ullamcorper. Vivamus varius rutrum leo, vitae ullamcorper diam aliquam et. Vivamus fringilla magna at quam efficitur, vitae convallis sapien rutrum. Duis ornare convallis nibh sit amet laoreet. Proin varius, elit quis mollis facilisis, dui tellus egestas est, auctor commodo eros enim in nisi. Etiam in massa nunc. Sed id ligula sit amet risus lacinia tincidunt. Sed pharetra arcu mi, at luctus metus viverra ut. Cras ex mi, porta quis vulputate a, lobortis nec tortor. In et ligula et diam tempor blandit in ut leo. Morbi accumsan convallis fringilla. Nam vel augue est.

Etiam eleifend sagittis vulputate. Aenean congue enim ac sem scelerisque, vel hendrerit leo facilisis. Vivamus aliquet faucibus congue. Aliquam sit amet sem porttitor.";

    [Benchmark(Baseline = true)]
    public string DotNetStringBuilder()
    {
        var builder = new System.Text.StringBuilder();
        builder.Append(Text);
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilder()
    {
        var builder = new ValueStringBuilder();
        builder.Append(Text);
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        builder.Replace("arcu", "some long word");
        builder.Replace("some long word", "arcu");
        return builder.ToString();
    }
}