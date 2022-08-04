Console.WriteLine("reads a php config file and replaces a string by a new one");
Console.WriteLine("");
Console.WriteLine("Syntax:");
Console.WriteLine("PatchPhpConfigFile [filename] [old string]  [new string]");
Console.WriteLine("");
Console.WriteLine("Example:");
Console.WriteLine("PatchPhpConfigFile config.php \"stringtoreplace\" \"replacebythis\"");


Console.WriteLine($"reading the file named '{args[0]}'");
var oldstring = args[1].Trim('\"');
var newstring = args[2].Trim('\"');
Console.WriteLine($"replacing {oldstring} by {newstring}");


var contents = File.ReadAllText(args[0]);

contents = contents.Replace(oldstring, newstring);

File.WriteAllText(args[0], contents);

Console.WriteLine("saved the changed file");
