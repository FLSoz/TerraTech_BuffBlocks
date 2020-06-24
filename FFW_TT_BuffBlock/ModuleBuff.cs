﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FFW_TT_BuffBlock
{
    public class ModuleBuff : Module
    {
        private void OnAttach()
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            buff.AddBuff(this);

        }

        private void OnDetach()
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            buff.RemoveBuff(this);
        }

        private void OnPool()
        {
            base.block.AttachEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachEvent.Subscribe(new Action(this.OnDetach));
        }

        [SerializeField]
        public string m_BuffType;

        [SerializeField]
        public float m_Strength;
    }
}