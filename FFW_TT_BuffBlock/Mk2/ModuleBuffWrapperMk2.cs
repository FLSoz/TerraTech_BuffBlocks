using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    internal class ModuleBuffWrapperMk2 : Module
    {
        private void OnAttach()
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.AddBlock(base.block);
        }

        private void OnDetach()
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.RemoveBlock(base.block);
        }

        private void OnPool()
        {
            base.block.AttachedEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachingEvent.Subscribe(new Action(this.OnDetach));
        }

        internal void PrintDetails() {
            BuffBlocks.logger.Debug($"Added ModuleBuffWrapperMk2 to block {base.block.name}");
        }
    }
}
