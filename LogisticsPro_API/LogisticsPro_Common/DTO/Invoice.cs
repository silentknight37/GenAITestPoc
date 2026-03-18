using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticsPro_Common.DTO
{
    public class Invoice
    {
        public Invoice()
        {
            InvoiceLineItems = new LineGenerateItem();
            LinkedProformaInvoices = new List<ProformaInvoice>();
        }
        public int Id { get; set; }
        public string InvoiceCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string CustomerTrn { get; set; }
        public string StatusName { get; set; }
        public string TransportDescription { get; set; }
        public string HotelDescription { get; set; }
        public string VisaDescription { get; set; }
        public string MiscellaneousDescription { get; set; }
        public string? City { get; set; }
        public int? CountryCode { get; set; }
        public int StatusId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? InvoiceDueDate { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public LineGenerateItem InvoiceLineItems { get; set; }
        public List<ProformaInvoice> LinkedProformaInvoices { get; set; }
        public List<string> TransportJobCardNos
        {
            get
            {
                return InvoiceLineItems.Transportations.Any() ? InvoiceLineItems.Transportations.Select(i => i.JobCardNo).Distinct().ToList() : new List<string>();
            }
        }

        public List<string> VisaJobCardNos
        {
            get
            {
                return InvoiceLineItems.Visa.Any() ? InvoiceLineItems.Visa.Select(i => i.JobCardNo).Distinct().ToList() : new List<string>();
            }
        }

        public List<string> HotelJobCardNos
        {
            get
            {
                return InvoiceLineItems.Hotels.Any() ? InvoiceLineItems.Hotels.Select(i => i.JobCardNo).Distinct().ToList() : new List<string>();
            }
        }

        public List<string> MiscellaneousJobCardNos
        {
            get
            {
                return InvoiceLineItems.Miscellaneous.Any() ? InvoiceLineItems.Miscellaneous.Select(i => i.JobCardNo).Distinct().ToList() : new List<string>();
            }
        }

        public decimal TotalLinkedProformaReciptAmount
        {
            get
            {
                var totalReceiptAmount = LinkedProformaInvoices.Select(i => i.TotalReciptAmount).Sum();

                return totalReceiptAmount;
            }
        }

        public string LinkProformaInvoiceCodes
        {
            get
            {
                var invoiceCodes = LinkedProformaInvoices.Select(i => i.InvoiceCode).ToList();

                return string.Join(",", invoiceCodes);
            }
        }

        public decimal InvoiceNetAmount
        {
            get
            {
                var transpotTotalSellPrice = InvoiceLineItems.Transportations.Select(i => i.TotalSellPrice).Sum();
                var transpotTotalSellTaxAmount = InvoiceLineItems.Transportations.Select(i => i.TotalSellTaxAmount).Sum();
                var transportNetAmount = transpotTotalSellPrice - transpotTotalSellTaxAmount;

                var hotelsTotalSellPrice = InvoiceLineItems.Hotels.Where(i => i.SellBaseAmount.HasValue).Select(i => i.SellBaseAmount).Sum();
                var visaTotalSellPrice = InvoiceLineItems.Visa.Where(i => i.SellBaseAmount.HasValue).Select(i => i.SellBaseAmount).Sum();
                var miscellaneousTotalSellPrice = InvoiceLineItems.Miscellaneous.Where(i => i.SellBaseAmount.HasValue).Select(i => i.SellBaseAmount).Sum();

                return transportNetAmount + hotelsTotalSellPrice.Value + visaTotalSellPrice.Value + miscellaneousTotalSellPrice.Value;
            }
        }

        public decimal InvoiceVatAmount
        {
            get
            {
                var transpotTotalSellTaxAmount = InvoiceLineItems.Transportations.Select(i => i.TotalSellTaxAmount).Sum();

                var hotelsTotalSellPrice = InvoiceLineItems.Hotels.Where(i=>i.SellTaxAmount.HasValue).Select(i => i.SellTaxAmount).Sum();
                var visaTotalSellPrice = InvoiceLineItems.Visa.Where(i => i.SellTaxAmount.HasValue).Select(i => i.SellTaxAmount).Sum();
                var miscellaneousTotalSellPrice = InvoiceLineItems.Miscellaneous.Where(i => i.SellTaxAmount.HasValue).Select(i => i.SellTaxAmount).Sum();

                return transpotTotalSellTaxAmount + hotelsTotalSellPrice.Value + visaTotalSellPrice.Value + miscellaneousTotalSellPrice.Value;
            }
        }

        public decimal InvoiceGrossAmount
        {
            get
            {
                var transpotTotalSellPrice = InvoiceLineItems.Transportations.Select(i => i.TotalSellPrice).Sum();

                var hotelsTotalSellPrice = InvoiceLineItems.Hotels.Select(i => i.TotalSellPrice).Sum();
                var visaTotalSellPrice = InvoiceLineItems.Visa.Select(i => i.TotalSellPrice).Sum();
                var miscellaneousTotalSellPrice = InvoiceLineItems.Miscellaneous.Select(i => i.TotalSellPrice).Sum();

                return transpotTotalSellPrice + hotelsTotalSellPrice + visaTotalSellPrice + miscellaneousTotalSellPrice;
            }
        }

        public string JobCards
        {
            get
            {
                List<string> jobCardList= new List<string>();
                jobCardList.AddRange(InvoiceLineItems.Transportations.Select(i => i.JobCardNo).ToList());
                jobCardList.AddRange(InvoiceLineItems.Hotels.Select(i => i.JobCardNo).ToList());
                jobCardList.AddRange(InvoiceLineItems.Visa.Select(i => i.JobCardNo).ToList());
                jobCardList.AddRange(InvoiceLineItems.Miscellaneous.Select(i => i.JobCardNo).ToList());

                return string.Join(',', jobCardList);
            }
        }
    }
}
