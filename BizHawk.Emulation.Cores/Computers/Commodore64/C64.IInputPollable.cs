﻿using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64
{
	public partial class C64// : IInputPollable
	{
		public bool IsLagFrame
		{
			get { return _isLagFrame; }
			set { _isLagFrame = value; }
		}

		public int LagCount
		{
			get { return _lagCount; }
			set { _lagCount = value; }
		}

		[SaveState.DoNotSave]
		public IInputCallbackSystem InputCallbacks { get; private set; }

		private bool _isLagFrame;
		private int _lagCount;
	}
}
