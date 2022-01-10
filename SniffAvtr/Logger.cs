using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SniffAvtr
{
	class Logger
	{
		#region Privates
		private static StackFrame CallerFrame;
		private static readonly ConcurrentQueue<string> Messages = new ConcurrentQueue<string>();
		private static readonly AutoResetEvent Trigger = new AutoResetEvent(false);
		private static readonly TextWriter Writer = File.AppendText("output.log");

		private static void ProcessQueue()
		{
			while (Trigger.WaitOne())
			{
				while (Messages.TryDequeue(out string message))
				{
					if (Debugger.IsAttached)
						Debugger.Log(0, "", message);
					Writer.Write(message);
				}
				Writer.Flush();
			}
		}

		private static readonly Thread LoggerThread = new Thread(new ThreadStart(ProcessQueue)) { IsBackground = true, Name = "Logger Thread" };

		static Logger()
		{
			LoggerThread.Start();
		}

		private static void SetCaller()
		{
			if (CallerFrame == null)
			{
				CallerFrame = new StackFrame(2);
			}
		}
		#endregion

		internal static void Flush()
		{
			Writer.Flush();
		}

		internal static void Write(string message)
		{
			SetCaller();
			Messages.Enqueue($"{DateTime.Now} [{CallerFrame.GetMethod().DeclaringType.Name}] {message}");
			Trigger.Set();
			CallerFrame = null;
		}

		internal static void WriteLine(string message)
		{
			SetCaller();
			Write(message + "\r\n");
			CallerFrame = null;
		}
	}
}
