﻿using BizHawk.Common.NumberExtensions;
using BizHawk.Emulation.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BizHawk.Emulation.Cores.Consoles.Vectrex
{
	public partial class VectrexHawk : IEmulator, IVideoProvider
	{
		public IEmulatorServiceProvider ServiceProvider { get; }

		public ControllerDefinition ControllerDefinition => _controllerDeck.Definition;

		public bool FrameAdvance(IController controller, bool render, bool rendersound)
		{
			if (_tracer.Enabled)
			{
				cpu.TraceCallback = s => _tracer.Put(s);
			}
			else
			{
				cpu.TraceCallback = null;
			}

			_frame++;

			if (controller.IsPressed("Power"))
			{
				HardReset();
			}

			_islag = true;

			// button inputs go to port 14 in the audio registers
			audio.Register[14] = (byte)(_controllerDeck.ReadPort1(controller) & 0xF);
			audio.Register[14] |= (byte)(_controllerDeck.ReadPort2(controller) << 4);

			frame_end = false;

			do_frame();

			if (_islag)
			{
				_lagcount++;
			}

			return true;
		}

		public void do_frame()
		{
			_vidbuffer = new int[VirtualWidth * VirtualHeight];

			//for (int i = 0; i < 1000; i++)
			while (!frame_end)
			{
				internal_state_tick();
				audio.tick();
				ppu.tick();
				cpu.ExecuteOne();				
			}
		}

		public int Frame => _frame;

		public string SystemId => "VEC"; 

		public bool DeterministicEmulation { get; set; }

		public void ResetCounters()
		{
			_frame = 0;
			_lagcount = 0;
			_islag = false;
		}

		public CoreComm CoreComm { get; }

		public void Dispose()
		{
			audio.DisposeSound();
		}

		#region Video provider

		public int _frameHz = 50;

		public int[] _vidbuffer;

		public int[] GetVideoBuffer()
		{
			return _vidbuffer;		
		}

		public int VirtualWidth => 256;
		public int VirtualHeight => 384;
		public int BufferWidth => 256;
		public int BufferHeight => 384;
		public int BackgroundColor => unchecked((int)0xFF000000);
		public int VsyncNumerator => _frameHz;
		public int VsyncDenominator => 1;

		#endregion
	}
}
