using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
namespace SamSoarII.LadderInstViewModel
{
    public class LadderInstViewModelPrototype
    {
        private static Dictionary<int, BaseViewModel> _elementCatalog = new Dictionary<int, BaseViewModel>();

        private static Dictionary<string, BaseViewModel> _elementName = new Dictionary<string, BaseViewModel>();

        static LadderInstViewModelPrototype()
        {
            Initialize();
        }

        private static void Initialize()
        {
            Add(new HorizontalLineViewModel());
            Add(new VerticalLineViewModel());
            //
            Add(new LDViewModel());
            Add(new LDIViewModel());
            Add(new LDIMViewModel());
            Add(new LDIIMViewModel());
            Add(new LDPViewModel());
            Add(new LDFViewModel());
            Add(new MEPViewModel());
            Add(new MEFViewModel());
            Add(new INVViewModel());
            Add(new OUTViewModel());
            Add(new OUTIMViewModel());
            Add(new SETViewModel());
            Add(new SETIMViewModel());
            Add(new RSTViewModel());
            Add(new RSTIMViewModel());
            Add(new ALTViewModel());
            Add(new ALTPViewModel());
            //
            Add(new LDWEQViewModel());
            Add(new LDWNEViewModel());
            Add(new LDWGEViewModel());
            Add(new LDWLEViewModel());
            Add(new LDWGViewModel());
            Add(new LDWLViewModel());

            Add(new LDDEQViewModel());
            Add(new LDDNEViewModel());
            Add(new LDDGEViewModel());
            Add(new LDDLEViewModel());
            Add(new LDDGViewModel());
            Add(new LDDLViewModel());

            Add(new LDFEQViewModel());
            Add(new LDFNEViewModel());
            Add(new LDFGEViewModel());
            Add(new LDFLEViewModel());
            Add(new LDFGViewModel());
            Add(new LDFLViewModel());

            //
            Add(new WTODViewModel());
            Add(new DTOWViewModel());
            Add(new DTOFViewModel());
            Add(new BINViewModel());
            Add(new BCDViewModel());
            Add(new ROUNDViewModel());
            Add(new TRUNCViewModel());
            //
            Add(new INVWViewModel());
            Add(new INVDViewModel());
            Add(new ANDWViewModel());
            Add(new ANDDViewModel());
            Add(new ORWViewModel());
            Add(new ORDViewModel());
            Add(new XORWViewModel());
            Add(new XORDViewModel());
            //
            Add(new MOVDViewModel());
            Add(new MOVViewModel());
            Add(new MOVFViewModel());
            Add(new MVBLKViewModel());
            Add(new MVDBLKViewModel());
            //
            Add(new ADDFViewModel());
            Add(new SUBFViewModel());
            Add(new MULFViewModel());
            Add(new DIVFViewModel());
            Add(new SQRTViewModel());
            Add(new SINViewModel());
            Add(new COSViewModel());
            Add(new TANViewModel());
            Add(new LNViewModel());
            Add(new EXPViewModel());
            //
            Add(new ADDViewModel());
            Add(new ADDDViewModel());
            Add(new SUBViewModel());
            Add(new SUBDViewModel());
            Add(new MULViewModel());
            Add(new MULWViewModel());
            Add(new MULDViewModel());
            Add(new DIVViewModel());
            Add(new DIVWViewModel());
            Add(new DIVDViewModel());
            Add(new INCViewModel());
            Add(new INCDViewModel());
            Add(new DECViewModel());
            Add(new DECDViewModel());
            //
            Add(new FORViewModel());
            Add(new NEXTViewModel());
            Add(new JMPViewModel());
            Add(new LBLViewModel());
            Add(new CALLViewModel());
            //
            Add(new SHLViewModel());
            Add(new SHLDViewModel());
            Add(new SHRViewModel());
            Add(new SHRDViewModel());
            Add(new ROLViewModel());
            Add(new ROLDViewModel());
            Add(new RORViewModel());
            Add(new RORDViewModel());
            //
            Add(new TRDViewModel());
            Add(new TWRViewModel());
        }

        private static void Add(BaseViewModel viewmodel)
        {
            _elementCatalog.Add(viewmodel.GetCatalogID(), viewmodel);
            _elementName.Add(viewmodel.InstructionName, viewmodel);
        }

        public static IEnumerable<BaseViewModel> GetElementViewModels()
        {
            return _elementCatalog.Values.Where(x => { return x.Type == ElementType.Input || x.Type == ElementType.Output || x.Type == ElementType.Special; });
        }

        public static BaseViewModel Clone(int id)
        {
            return _elementCatalog[id].Clone();
        }

        public static BaseViewModel Clone(string instName)
        {
            return _elementName[instName].Clone();
        }

    }
}


