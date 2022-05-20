using LuccaDevises.Tools;

args = new string[1] { "./FilesFolder/Sample.csv"};             // TODO : supprimer.

if (args.Length != 1)
{
    Console.Error.WriteLine("Un chemin de fichier doit être spécifier.");
}
else
{
    // Define the cancellation token.
    CancellationTokenSource source = new CancellationTokenSource();
    CancellationToken token = source.Token;

    try
    {
        var fileParser = new FileParser(args[0]);
        await foreach (string currentLine in fileParser.ReadContent(token))
        {
            Console.WriteLine(currentLine);
        }
    }
    catch(Exception exc)
    {
        source.Cancel();
        Console.Error.WriteLine($"Le fichier fourni n'a pas un format valide => {exc.Message}");
    }
    finally
    {
        source.Dispose();
    }
}

