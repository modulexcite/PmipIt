using System;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace PmipIt
{
	internal sealed class PmipItCommand
	{
		public const int CommandId = 0x0100;
		public static readonly Guid CommandSet = new Guid("de766436-9607-48b3-b78c-4ab8f6ceb849");

		private readonly PmipItPackage _package;

		private PmipItCommand(PmipItPackage package)
		{
			if (package == null)
				throw new ArgumentNullException();

			_package = package;

			var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (commandService == null)
				return;

			var menuCommandId = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
			commandService.AddCommand(menuItem);
		}

		public static PmipItCommand Instance
		{
			get;
			private set;
		}

		private IServiceProvider ServiceProvider => _package;

		public static void Initialize(PmipItPackage package)
		{
			Instance = new PmipItCommand(package);
		}

		private void MenuItemCallback(object sender, EventArgs e)
		{
			var dte = _package.Dte;

			var debugger = dte.Debugger;
			if (debugger == null)
				return;

			var thread = dte.Debugger.CurrentThread;

			var frames = thread?.StackFrames;
			if (frames == null)
				return;

			_package.OutputWindowDebugPane.Activate();

			foreach (StackFrame frame in frames)
			{
				var stackInfo = frame.FunctionName;
				if (Regex.IsMatch(stackInfo, "\\d+()"))
				{
					var expression = $"{{,,mono.dll}}mono_pmip((void*)0x{Regex.Replace(stackInfo, "\\(|\\)", string.Empty)})";
					var result = debugger.GetExpression(expression, true);

					if (!result.IsValidValue)
						stackInfo = string.Concat("Unable to evaluate ", expression);
					else if (!result.Value.EndsWith("<NULL>"))
						stackInfo = result.Value;
				}

				if (!string.IsNullOrEmpty(frame.Module))
				{
					_package.OutputWindowDebugPane.OutputString(frame.Module);
					_package.OutputWindowDebugPane.OutputString("!");
				}
				_package.OutputWindowDebugPane.OutputString(stackInfo);
				_package.OutputWindowDebugPane.OutputString(Environment.NewLine);
			}
		}
	}
}
