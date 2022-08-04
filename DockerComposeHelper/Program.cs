// See https://aka.ms/new-console-template for more information
Console.WriteLine("creates a CMD file to set an environment variable with the image of a container");
Console.WriteLine("reading the file named 'temp'");

var contents = File.ReadAllText("temp");
var parts = contents.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

var command = $"set CONTAINER_ID={parts[0]}";
File.WriteAllText("set_container_id.cmd", command);

Console.WriteLine("created a CMD file named 'set_container_id.cmd'");
