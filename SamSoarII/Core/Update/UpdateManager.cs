﻿using SamSoarII.Shell.Windows.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSoarII.Core.Update
{
    public class UpdateManager:IDisposable
    {
        private InteractionFacade ifParent;
        public InteractionFacade IFParent { get { return ifParent; } }
        public UpdateWindow View { get { return ifParent.WNDInform; } }

        private List<Information> informations = new List<Information>();
        public List<Information> Informations { get { return informations; } }

        public event EventHandler InformationsCountChanged = delegate { };

        public UpdateManager(InteractionFacade _ifParent)
        {
            ifParent = _ifParent;
        }
        public void ChildrensChanged()
        {
            InformationsCountChanged(this, new EventArgs());
        }
        public void AddInformation(Information information)
        {
            informations.Add(information);
            ChildrensChanged();
        }

        public void Dispose()
        {
            ifParent = null;
            InformationsCountChanged = delegate { };
            foreach (var information in informations)
            {
                information.Dispose();
            }
            informations = null;
        }
    }
}
