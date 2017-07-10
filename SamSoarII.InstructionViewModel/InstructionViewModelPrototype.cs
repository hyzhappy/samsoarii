using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamSoarII.LadderInstModel;
using SamSoarII.LadderInstViewModel.Counter;
using SamSoarII.LadderInstViewModel.Interrupt;
using SamSoarII.LadderInstViewModel;
using SamSoarII.LadderInstViewModel.Pulse;
using SamSoarII.LadderInstViewModel.Auxiliar;
using System.Windows.Threading;

namespace SamSoarII.LadderInstViewModel
{
    public class LadderInstViewModelPrototype
    {
        private static SortedList<int, BaseViewModel> _elementCatalog 
            = new SortedList<int, BaseViewModel>();
        private static Dictionary<string, BaseViewModel> _elementName 
            = new Dictionary<string, BaseViewModel>();
        private static IList<int> _elementCatalogList;

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
            Add(new TONViewModel());
            Add(new TOFViewModel());
            Add(new TONRViewModel());
            //
            Add(new CTUViewModel());
            Add(new CTDViewModel());
            Add(new CTUDViewModel());
            //
            Add(new FORViewModel());
            Add(new NEXTViewModel());
            Add(new JMPViewModel());
            Add(new LBLViewModel());
            Add(new CALLViewModel());
            Add(new CALLMViewModel());
            //
            Add(new SHLViewModel());
            Add(new SHLDViewModel());
            Add(new SHRViewModel());
            Add(new SHRDViewModel());
            Add(new ROLViewModel());
            Add(new ROLDViewModel());
            Add(new RORViewModel());
            Add(new RORDViewModel());
            Add(new SHLBViewModel());
            Add(new SHRBViewModel());
            //
            Add(new ATCHViewModel());
            Add(new DTCHViewModel());
            Add(new EIViewModel());
            Add(new DIViewModel());
            //
            Add(new TRDViewModel());
            Add(new TWRViewModel());
            //
            Add(new MBUSViewModel());
            Add(new SENDViewModel());
            Add(new REVViewModel());
            //
            Add(new PLSFViewModel());
            Add(new DPLSFViewModel());
            Add(new PWMViewModel());
            Add(new DPWMViewModel());
            Add(new PLSYViewModel());
            Add(new DPLSYViewModel());
            Add(new PLSRViewModel());
            Add(new DPLSRViewModel());
            Add(new PLSRDViewModel());
            Add(new DPLSRDViewModel());
            Add(new PLSNEXTViewModel());
            Add(new PLSSTOPViewModel());
            Add(new ZRNViewModel());
            Add(new DZRNViewModel());
            Add(new PTOViewModel());
            Add(new DRVIViewModel());
            Add(new DDRVIViewModel());
            //
            Add(new HCNTViewModel());
            //
            Add(new LOGViewModel());
            Add(new POWViewModel());
            Add(new FACTViewModel());
            Add(new CMPViewModel());
            Add(new CMPDViewModel());
            Add(new CMPFViewModel());
            Add(new ZCPViewModel());
            Add(new ZCPDViewModel());
            Add(new ZCPFViewModel());
            Add(new NEGViewModel());
            Add(new NEGDViewModel());
            Add(new XCHViewModel());
            Add(new XCHDViewModel());
            Add(new XCHFViewModel());
            Add(new CMLViewModel());
            Add(new CMLDViewModel());
            Add(new SMOVViewModel());
            Add(new FMOVViewModel());
            Add(new FMOVDViewModel());

            _elementCatalogList = _elementCatalog.Keys.ToArray();
            
        }

        private static void Add(BaseViewModel viewmodel)
        {
            if (_elementCatalog.ContainsKey(viewmodel.GetCatalogID()))
            {
                Console.Write("Already contain catalogID {0:d} {1:s}\n",
                    viewmodel.GetCatalogID(), viewmodel.ToString());
            }
            else
            {
                _elementCatalog.Add(viewmodel.GetCatalogID(), viewmodel);
            }
            if (_elementName.ContainsKey(viewmodel.InstructionName))
            {
                Console.Write("Already contain Name {0:s} {1:s}\n",
                     viewmodel.InstructionName, viewmodel.ToString());
            }
            else
            {
                _elementName.Add(viewmodel.InstructionName, viewmodel);
            }
        }

        public static IEnumerable<BaseViewModel> GetElementViewModels()
        {
            return _elementCatalog.Values.Where(x => { return x.Type == ElementType.Input || x.Type == ElementType.Output || x.Type == ElementType.Special; });
        }
        public static bool CheckInstructionName(string InstructionName)
        {
            return _elementName.Keys.Contains(InstructionName);
        }
        public static int GetOrderFromCatalog(int catalog)
        {
            return _elementCatalog.IndexOfKey(catalog);
        }
        public static int GetCatalogFromOrder(int order)
        {
            return _elementCatalogList[order];
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


