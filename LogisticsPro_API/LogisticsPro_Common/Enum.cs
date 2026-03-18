using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common
{
    public enum EnumContextType
    {
        Transpotation=1,
        Hotel=2,
        Visa=3,
        Miscellaneous = 4,
    }

    public enum EnumStatus
    {
        Active = 1,
        Deactivated = 2
    }

    public enum EnumInvoiceStatus
    {
        Generated = 1,
        Sent = 2,
        Void=3,
        PartiallyPaid=4,
        Paid = 5
    }

    public enum EnumRole
    {
        FinanceAdmin = 1,
        OperationClark = 2
    }
}
