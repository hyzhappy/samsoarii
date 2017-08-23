﻿using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Shell.Models
{
    public class OutputRectVisualUnitModel : BaseVisualUnitModel, IResource
    {
        public override IResource Create(params object[] args)
        {
            return new OutputRectVisualUnitModel((LadderUnitModel)args[0]);
        }

        public override void Recreate(params object[] args)
        {
            base.Recreate(args);
            recreating = true;
            //TODO Render
            RenderAll();
            recreating = false;
        }

        public OutputRectVisualUnitModel(LadderUnitModel _core)
        {
            if (_core != null) Recreate(_core);
        }

        public override void Dispose()
        {
            base.Dispose();
            AllResourceManager.Dispose(this);
        }
    }
}