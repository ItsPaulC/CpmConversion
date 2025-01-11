using Cocona;
using System;

namespace CpmConverterCli;

class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    public void Run(
        [Option("pathToSln", Description = "Path to the solution file.")] string pathToSln = "",
        [Option("addEditorConfig", Description = "Flag to add .editorconfig file.")] bool addEditorConfig = false)
    {

        //
        pathToSln = "D:\\repos\\CmpConverter\\ConverToCpm\\ConverToCpm.sln";

        Console.WriteLine($"Path to Solution: {pathToSln}");
        PropsFactory propsFactory = new(pathToSln);
        Console.WriteLine($"Add .editorconfig: {addEditorConfig}");
    }
}