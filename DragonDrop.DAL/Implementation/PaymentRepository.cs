using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.DAL.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private DragonDrop_DbContext _context;

        public PaymentRepository(DragonDrop_DbContext context) => _context = context;

        public Payment Get(int id)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment with ID:\t" + id);
            var pay = _context.Payments.SingleOrDefault(c => c.PaymentId == id);
            if (pay == null) StaticLogger.LogWarn(GetType(), "Repo found no Payment with ID:\t" + id);
            return pay;
        }

        public void Create(Payment pay)
        {
            StaticLogger.LogInfo(GetType(), "Repo creating record for Payment:\t" + pay.PaymentId);

            if (pay.CustomerId == 0 || pay.Amount == null || pay.PaymentMethodId == 0 || pay.Date == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not register Payment because submitted info was incomplete.");
                return;
            }

            if (!_context.Customers.Any(c => c.CustomerId == pay.CustomerId))
            {
                StaticLogger.LogError(GetType(), "Repo could not register Payment because no customer with ID:\t." + pay.CustomerId + " was found.");
                return;
            }

            _context.Payments.Add(pay);
            _context.SaveChanges();
            StaticLogger.LogInfo(GetType(), "Payment:/t" + pay.PaymentId + " succesfully registered.");
        }

        public void Update(Payment pay)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for Payment with ID:\t" + pay.PaymentId);

            if (Get(pay.PaymentId) == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not find any Payment with ID:\t" + pay.PaymentId + " to update.");
                return;
            }

            var payToUpd = Get(pay.PaymentId);

            if (pay.CustomerId == 0 || pay.Amount == null || pay.PaymentMethodId == 0 || pay.Date == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not update Payment because submitted info was incomplete.");
                return;
            }

            if (!_context.Customers.Any(c => c.CustomerId == pay.CustomerId))
            {
                StaticLogger.LogError(GetType(), "Repo could not update Payment because no customer with ID:\t." + pay.CustomerId + " was found.");
                return;
            }

            payToUpd.Date = pay.Date;
            payToUpd.PaymentMethodId = pay.PaymentMethodId;
            payToUpd.CustomerId = pay.CustomerId;
            payToUpd.Amount = pay.Amount;
            _context.SaveChanges();
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Payment with ID:\t" + pay.PaymentId);
        }

        #region Gets

        public IEnumerable<Payment> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Payments...");
            var pays = _context.Payments.ToList();
            StaticLogger.LogInfo(GetType(), pays.Count().ToString() + " records found.");
            return pays;
        }

        public IEnumerable<Payment> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Payment> payList = null)
        {
            if (pgIndex == 0) return GetAll();
            pgIndex = Math.Abs(pgIndex);
            pgSize = Math.Abs(pgSize);

            var payScope = payList == null ? _context.Payments : payList;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from "+(payList == null ? "all":"select")+" Payment records.");

            var pays = payScope.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllSorted(string sortBy, bool desc = false, IEnumerable<Payment> payList = null)
        {
            var pays = new List<Payment>();
            var payScope = payList == null ? _context.Payments : payList;

            var log = new StringBuilder("Repo fetching ");
            if (payList == null) log.Append("all");
            log.Append(" Payments");
            if (payList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }

            if (sortBy == null) return payScope;
            switch (sortBy.Trim().ToUpper())
            {
                case "CUSTOMER":
                case "CUST":
                case "CUSTOMERID":
                case "CUSTID":
                case "CUSTOMER_ID":
                case "CUST_ID":
                    pays = desc ? payScope.OrderByDescending(p => p.CustomerId).ToList() :
                        payScope.OrderBy(p => p.CustomerId).ToList();
                    log.Append("Customer");
                    break;
                case "AMT":
                case "AMOUNT":
                    pays = desc ? payScope.OrderByDescending(p => p.Amount).ToList() :
                        payScope.OrderBy(p => p.Amount).ToList();
                    log.Append("Amount");
                    break;
                case "DATE":
                    pays = desc ? payScope.OrderByDescending(p => p.Date).ToList() :
                        payScope.OrderBy(p => p.Date).ToList();
                    log.Append("Date");
                    break;
                case "METHOD":
                case "METH":
                case "PAYMENTMETHOD":
                case "PAYMETHOD":
                case "PAYMENTMETH":
                case "PAYMETH":
                case "PAYMENT_METHOD":
                case "PAYMENT_METH":
                case "PAY_METHOD":
                case "PAY_METH":
                    pays = desc ? payScope.OrderByDescending(p => p.PaymentMethodId).ToList() :
                        payScope.OrderBy(p => p.PaymentMethodId).ToList();
                    log.Append("Payment Method");
                    break;
                default:
                    pays = desc ? payScope.OrderByDescending(p => p.PaymentId).ToList() :
                        payScope.OrderBy(p => p.PaymentId).ToList();
                    log.Append("ID");
                    break;
            }


            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), pays.Count().ToString() + " records found.");
            return pays;
        }

        #region Filters

        public IEnumerable<Payment> GetAllFiltered(string searchBy, IEnumerable<Payment> payList = null)
        {
            var payScope = payList == null ? _context.Payments : payList;

            searchBy = searchBy.ToUpper();
            StaticLogger.LogInfo(GetType(), "Repo searching Payment records by:\t" + searchBy);
            var pays = new List<Payment>();

            var searchTerms = searchBy.Split(' ');
            var date = new DateTime(1600, 1, 1);
            var amount = 0m;
            foreach (var term in searchTerms)
            {
                var isDate = DateTime.TryParse(term, out date);
                var isAmount = decimal.TryParse(term, out amount);

                pays.AddRange
                     (
                         payScope.Where(p =>
                             isDate && p.Date == date ||
                             _context.PaymentMethods.Any(pm => pm.Name.ToUpper()
                             .Contains(term) && pm.PaymentMethodId == p.PaymentMethodId) ||
                             _context.Customers.Any(c => c.Name.ToUpper()
                             .Contains(term) && c.CustomerId == p.CustomerId) ||
                             isAmount && p.Amount == amount)
                     );
            }

            pays = pays.Distinct().ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + pays.Count() + " Payments that match: " + searchBy);
            return pays.OrderBy(p=>p.PaymentId);
        }

        public IEnumerable<Payment> GetAllFiltered(DateTime when, bool before, IEnumerable<Payment> payList = null)
        {
            var payScope = payList == null ? _context.Payments : payList;

            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Repo searching Payments placed " + befAftText + ":\t" + when.ToShortDateString());
            var pays = before ? payScope.Where(p => p.Date < when).ToList() :
                                payScope.Where(p => p.Date > when).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + pays.Count() + " Payments that match criteria.");
            return pays.OrderBy(p => p.PaymentId);

        }

        public IEnumerable<Payment> GetAllFiltered(decimal amount, bool over, IEnumerable<Payment> payList = null)
        {
            var payScope = payList == null ? _context.Payments : payList;

            var smlGrtText = over ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Repo searching Payments " + smlGrtText + ":\t" + amount + "$.");
            var pays = over ? payScope.Where(p => p.Amount > amount).ToList():
                              payScope.Where(p => p.Amount < amount).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + pays.Count() + " Payments that match criteria.");
            return pays.OrderBy(p => p.PaymentId);

        }

        public IEnumerable<Payment> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex, IEnumerable<Payment> payList = null)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Payments, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllFiltered(searchBy, payList));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndPaged(DateTime when, bool before, int pgSize, int pgIndex, IEnumerable<Payment> payList = null)
        {
            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Payments, according to \n DATE: " + befAftText + " " + when + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllFiltered(when, before, payList));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndPaged(decimal amount, bool over, int pgSize, int pgIndex, IEnumerable<Payment> payList = null)
        {
            var smlGrtText = over ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Payments, according to \n Amount: " + smlGrtText + " than " + amount + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllFiltered(amount, over, payList));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        #endregion

        public IEnumerable<Payment> GetAllByCustomerId(int custId)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching all Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = _context.Payments.Where(p => p.CustomerId == custId).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records of payments by Customer ID: " + custId);
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndSorted(int custId, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retreiving Payments made by Customer:\t" + custId + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " and sorting them in "+ (desc?"descending":"ascending") +" order, according to criteria: "+sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = GetAllSorted(sortBy, desc, GetAllByCustomerId(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retreiving Payments made by Customer:\t" + custId + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " from Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = GetAllByCustomerId(custId).Skip(pgSize * (pgIndex - 1)).Take(pgSize);
            StaticLogger.LogInfo(GetType(), "Repo found: " + pays.Count() + " Payments on requested page.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, string searchBy)
        {
            try
            {
                StaticLogger.LogInfo(GetType(),"Repo fetching all Payments made by: " + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " and filtering them by: " + searchBy + ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = GetAllFiltered(searchBy, GetAllByCustomerId(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved :" + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, DateTime when, bool before)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching all Payments made by: " + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + (before ? "before":"after")+" " + when.ToString()+ ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = GetAllFiltered(when, before, GetAllByCustomerId(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved :" + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, decimal amount, bool over)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + (over ? "over" : "under") + " " + amount.ToString("{0:0.##}"));
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllFiltered(amount, over, GetAllByCustomerId(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdSortedAndPaged
            (int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", Sorting them in "+(desc?"descending":"ascending")+" order by: "+sortBy+" and displaying Page: "+pgIndex+" of Size: "+pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllPaginated(pgSize,pgIndex, GetAllByCustomerIdAndSorted(custId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", filtering by: "+searchBy+" and showing Page: "+pgIndex +" of Size: "+pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllPaginated(pgSize,pgIndex, GetAllByCustomerIdAndFiltered(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, DateTime when, bool before, int pgSize, int pgIndex)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " " + (before?"before":"after")+" "+when.ToShortDateString()+ " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdAndFiltered(custId, when, before));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, decimal amount, bool over, int pgSize, int pgIndex)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " "+(over?"over": "under") +" "+amount.ToString("{0:0.##}") + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdAndFiltered(custId, amount, over));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId,string searchBy , string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", Filtering by: " + searchBy + " and Sorting in "+ (desc?"descending":"ascending") +" order by: " + sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllSorted(sortBy, desc, GetAllByCustomerIdAndFiltered(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }        

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, DateTime when, bool before, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments "+(before?"before":"after") + " "+when.ToShortDateString()+" made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " and Sorting in "+ (desc?"descending":"ascending") +" order by: " + sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllSorted(sortBy, desc, GetAllByCustomerIdAndFiltered(custId, when, before));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }        

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, decimal amount, bool over, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments " + (over?"over":"under") + " " + amount.ToString("{0:0.##}") + " made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " and Sorting in " + (desc ? "descending" : "ascending") + " order by: " + sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllSorted(sortBy, desc, GetAllByCustomerIdAndFiltered(custId, amount, over));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records, sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order, and displaying Page " + pgIndex + " of Size: " + pgSize + ".");
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllSorted(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering Payments by: " + searchBy + " and sorting them in " + (desc ? "descending" : "ascending") + " order by " + sortBy + ".");

            var pays = GetAllSorted(sortBy, desc, GetAllFiltered(searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndSorted(DateTime when, bool before, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo getting Payments " + (before?"before":"after")+" "+ when.ToShortDateString() + " and sorting them in " + (desc ? "descending" : "ascending") + " order by " + sortBy + ".");

            var pays = GetAllSorted(sortBy, desc, GetAllFiltered(when, before));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndSorted(decimal amount, bool over, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering " + (over?"over":"under")+amount.ToString("{0:0.##}") + " and sorting them in " + (desc ? "descending" : "ascending") + " order by " + sortBy + ".");

            var pays = GetAllSorted(sortBy, desc, GetAllFiltered(amount, over));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredSortedAndPaged
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc=false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records filtered by: "+ searchBy+", then sorting them in "+(desc?"descending":"asending")+" order by: "+ sortBy+" and showing Page: "+ pgIndex+" of Size: "+pgSize);
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredSortedAndPaged
            (DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records " + (before?"before":"after")+" "+when.ToShortDateString() + ", then sorting them in " + (desc ? "descending" : "asending") + " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(when, before, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredSortedAndPaged
            (decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records " + (over ? "over": "under") + " " + amount.ToString("{0:0.##}")+ ", then sorting them in " + (desc ? "descending" : "asending") + " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(amount, over, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged
            (int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " Filtering by: " + searchBy + ", Sorting in " +(desc?"descending":"ascending") +" order by: "+ sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdFilteredAndSorted(custId, searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged
            (int custId, DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made "+(before?"before":"after")+" "+when.ToShortDateString()+" by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", Sorting in " +(desc?"descending":"ascending") +" order by: "+ sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdFilteredAndSorted(custId, when, before, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged
            (int custId, decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments " + (over ? "over": "under" ) + " " + amount.ToString("{0:0.##}") + " made by Customer:\t" + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", Sorting in " + (desc ? "descending" : "ascending") + " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdFilteredAndSorted(custId, amount, over, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        #endregion

        public bool PaymentIdExists(int payId) => _context.Payments.Any(p => p.PaymentId == payId);

        public bool CustomerIdExists(int custId) => _context.Customers.Any(c => c.CustomerId == custId);

        public bool PaymentMethodIdExists(int methId) => _context.PaymentMethods.Any(m => m.PaymentMethodId == methId);

        #region Async

        public async Task CreateAsync(Payment pay)
        {
            StaticLogger.LogInfo(GetType(), "Repo creating record for Payment:\t" + pay.PaymentId);

            if (pay.CustomerId == 0 || pay.Amount == null || pay.PaymentMethodId == 0 || pay.Date == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not register Payment because submitted info was incomplete.");
                return;
            }

            if (await _context.Customers.FindAsync(pay.CustomerId)==null)
            {
                StaticLogger.LogError(GetType(), "Repo could not register Payment because no customer with ID:\t." + pay.CustomerId + " was found.");
                return;
            }

            _context.Payments.Add(pay);
            await _context.SaveChangesAsync();
            StaticLogger.LogInfo(GetType(), "Payment:/t" + pay.PaymentId + " succesfully registered.");
        }

        public async Task UpdateAsync(Payment pay)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for Payment with ID:\t" + pay.PaymentId);

            if ((await GetAsync(pay.PaymentId)) == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not find any Payment with ID:\t" + pay.PaymentId + " to update.");
                return;
            }

            var payToUpd = await GetAsync(pay.PaymentId);

            if (pay.CustomerId == 0 || pay.Amount == null || pay.PaymentMethodId == 0 || pay.Date == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not update Payment because submitted info was incomplete.");
                return;
            }

            if ((await _context.Customers.Where(c => c.CustomerId == pay.CustomerId).ToListAsync()).Count == 0)
            {
                StaticLogger.LogError(GetType(), "Repo could not update Payment because no customer with ID:\t." + pay.CustomerId + " was found.");
                return;
            }

            payToUpd.Date = pay.Date;
            payToUpd.PaymentMethodId = pay.PaymentMethodId;
            payToUpd.CustomerId = pay.CustomerId;
            payToUpd.Amount = pay.Amount;
            await _context.SaveChangesAsync();
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Payment with ID:\t" + pay.PaymentId);
        }

        #region Gets

        public async Task<Payment> GetAsync(int id)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment with ID:\t" + id);
            var pay = await _context.Payments.FindAsync(id);
            if (pay == null) StaticLogger.LogWarn(GetType(), "Repo found no Payment with ID:\t" + id);
            return pay;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Payments...");
            var pays = await _context.Payments.ToListAsync();
            StaticLogger.LogInfo(GetType(), pays.Count().ToString() + " records found.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllPaginatedAsync
            (int pgSize, int pgIndex, IEnumerable<Payment> payList=null)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from all Payment records.");
            var pays = new List<Payment>();
            if (payList!=null) pays = payList.Skip(pgSize * (pgIndex - 1))
                    .Take(pgSize).ToList();
            else pays= await _context.Payments.Skip(pgSize * (pgIndex - 1))
                    .Take(pgSize).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAsync
            (string searchBy, IEnumerable<Payment> payList = null)
        {
            searchBy = searchBy.ToUpper();
            StaticLogger.LogInfo(GetType(), "Repo searching Payment records by:\t" + searchBy);
            if (payList != null) return GetAllFiltered(searchBy, payList);
            var pays = new List<Payment>();

            var searchTerms = searchBy.Split(' ');
            var date = new DateTime(1600, 1, 1);
            var amount = 0m;
            foreach (var term in searchTerms)
            {
                var isDate = DateTime.TryParse(term, out date);
                var isAmount = decimal.TryParse(term, out amount);

                pays.AddRange
                     (await
                         _context.Payments.Where(p =>
                             isDate && p.Date == date ||
                             _context.PaymentMethods.Any(pm => pm.Name.ToUpper()
                             .Contains(term) && pm.PaymentMethodId == p.PaymentMethodId) ||
                             _context.Customers.Any(c => c.Name.ToUpper()
                             .Contains(term) && c.CustomerId == p.CustomerId) ||
                             isAmount && p.Amount == amount).ToListAsync()
                     );
            }

            pays = pays.Distinct().ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + pays.Count() + " Payments that match: " + searchBy);
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAsync
            (DateTime when, bool before, IEnumerable<Payment> payList = null)
        {
            if (payList != null) return GetAllFiltered(when, before, payList);

            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Repo searching Payments placed " + befAftText + ":\t" + when.ToShortDateString());
            var pays = before ? await _context.Payments.Where(p => p.Date < when).ToListAsync() :
                                await _context.Payments.Where(p => p.Date > when).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + pays.Count() + " Payments that match criteria.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> 
            GetAllFilteredAsync(decimal amount, bool over, IEnumerable<Payment> payList = null)
        {
            if (payList != null) return GetAllFiltered(amount, over, payList);

            var smlGrtText = over ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Repo searching Payments " + smlGrtText + ":\t" + amount + "$.");
            var pays = over ? await _context.Payments.Where(p => p.Amount > amount).ToListAsync() :
                                await _context.Payments.Where(p => p.Amount < amount).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + pays.Count() + " Payments that match criteria.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllSortedAsync
            (string sortBy, bool desc = false, IEnumerable<Payment> payList = null)
        {
            if (payList != null) return await GetAllSortedAsync(sortBy, desc);
            var pays = new List<Payment>();

            var log = new StringBuilder("Repo fetching ");
            if (payList == null) log.Append("all");
            log.Append(" Payments");
            if (payList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }

            if (sortBy == null) return await _context.Payments.ToListAsync();
            switch (sortBy.Trim().ToUpper())
            {
                case "CUSTOMER":
                case "CUST":
                case "CUSTOMERID":
                case "CUSTID":
                case "CUSTOMER_ID":
                case "CUST_ID":
                    pays = desc ? await _context.Payments.OrderByDescending(p => p.CustomerId).ToListAsync() :
                        await _context.Payments.OrderBy(p => p.CustomerId).ToListAsync();
                    log.Append("Customer");
                    break;
                case "AMT":
                case "AMOUNT":
                    pays = desc ? await _context.Payments.OrderByDescending(p => p.Amount).ToListAsync() :
                        await _context.Payments.OrderBy(p => p.Amount).ToListAsync();
                    log.Append("Amount");
                    break;
                case "DATE":
                    pays = desc ? await _context.Payments.OrderByDescending(p => p.Date).ToListAsync() :
                        await _context.Payments.OrderBy(p => p.Date).ToListAsync();
                    log.Append("Date");
                    break;
                case "METHOD":
                case "METH":
                case "PAYMENTMETHOD":
                case "PAYMETHOD":
                case "PAYMENTMETH":
                case "PAYMETH":
                case "PAYMENT_METHOD":
                case "PAYMENT_METH":
                case "PAY_METHOD":
                case "PAY_METH":
                    pays = desc ? await _context.Payments.OrderByDescending(p => p.PaymentMethodId).ToListAsync() :
                        await _context.Payments.OrderBy(p => p.PaymentMethodId).ToListAsync();
                    log.Append("Payment Method");
                    break;
                default:
                    pays = desc ? await _context.Payments.OrderByDescending(p => p.PaymentId).ToListAsync() :
                        await _context.Payments.OrderBy(p => p.PaymentId).ToListAsync();
                    log.Append("ID");
                    break;
            }

            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), pays.Count().ToString() + " records found.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, IEnumerable<Payment> payList = null)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Payments, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Payment records.");
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex,
                  await GetAllFilteredAsync(searchBy,
                  payList == null ? await _context.Payments.ToListAsync() : payList));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (DateTime when, bool before, int pgSize, int pgIndex, IEnumerable<Payment> payList = null)
        {
            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Payments, according to \n DATE: " + befAftText + " " + when + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Payment records.");
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex,
                  await GetAllFilteredAsync(when, before,
                  payList == null ? await _context.Payments.ToListAsync() : payList));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (decimal amount, bool smaller, int pgSize, int pgIndex, IEnumerable<Payment> payList = null)
        {
            var smlGrtText = smaller ? "under" : "over";
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Payments, according to \n Amount: " + smlGrtText + " than " + amount + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Payment records.");
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex,
                   await GetAllFilteredAsync(amount, smaller,
                   payList == null ? await _context.Payments.ToListAsync() : payList));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count().ToString() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAsync(int custId)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching all Payments made by Customer:\t" + (await _context.Customers.FindAsync(custId)).Name);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await _context.Payments.Where(p => p.CustomerId == custId).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records of payments by Customer ID: " + custId);
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, string searchBy)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching all Payments made by: " +
                    name.Name + " and filtering them by: " + searchBy + ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = await GetAllFilteredAsync(searchBy,
                       await GetAllByCustomerIdAsync(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved :" + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, DateTime when, bool before)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching all Payments made by: " + name.Name + (before ? "before" : "after") + " " + when.ToString() + ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = await GetAllFilteredAsync(when, before, 
                       await GetAllByCustomerIdAsync(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved :" + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, decimal amount, bool over)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + 
                    name.Name + (over ? "over" : "under") + " " + 
                    amount.ToString("{0:0.##}"));
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllFilteredAsync(amount, over, 
                       await GetAllByCustomerIdAsync(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndSortedAsync
            (int custId, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo retreiving Payments made by Customer:\t" + 
                    custId + "\tNAME: "+ name.Name + " and sorting them in " + 
                    (desc ? "descending" : "ascending") + " order, according to criteria: " + sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = await GetAllSortedAsync(sortBy, desc, 
                       await GetAllByCustomerIdAsync(custId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdSortedAndPagedAsync
            (int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" +
                    name.Name + ", Sorting them in " + (desc ? "descending" : "ascending") +
                    " order by: " + sortBy + " and displaying Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllByCustomerIdAndSortedAsync(custId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" +
                    name.Name + ", filtering by: " + searchBy + 
                    " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllByCustomerIdAndFilteredAsync(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, DateTime when, bool before, int pgSize, int pgIndex)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" + 
                    name.Name + " " + (before ? "before" : "after") + " " +
                    when.ToShortDateString() + " and showing Page: " + 
                    pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex,
                       await GetAllByCustomerIdAndFilteredAsync(custId, when, before));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, decimal amount, bool over, int pgSize, int pgIndex)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" +
                    name.Name + " " + (over ? "over" : "under") + " " + 
                    amount.ToString("{0:0.##}") + " and showing Page: " + 
                    pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex,
                       await GetAllByCustomerIdAndFilteredAsync(custId, amount, over));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, string searchBy, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" +
                    name.Name + ", Filtering by: " + searchBy + " and Sorting in " +
                    (desc ? "descending" : "ascending") + " order by: " + sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllSortedAsync(sortBy, desc, 
                       await GetAllByCustomerIdAndFilteredAsync(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, DateTime when, bool before, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments " + 
                    (before ? "before" : "after") + " " + when.ToShortDateString() + 
                    " made by Customer:\t" + name.Name + " and Sorting in " +
                    (desc ? "descending" : "ascending") + " order by: " + sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllSortedAsync(sortBy, desc,
                       await GetAllByCustomerIdAndFilteredAsync(custId, when, before));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, decimal amount, bool over, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments " + 
                    (over ? "over" : "under") + " " + amount.ToString("{0:0.##}") + 
                    " made by Customer:\t" + name.Name + " and Sorting in " +
                    (desc ? "descending" : "ascending") + " order by: " + sortBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllSortedAsync(sortBy, desc, 
                       await GetAllByCustomerIdAndFilteredAsync(custId, amount, over));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made by Customer:\t" +
                    name.Name + " Filtering by: " + searchBy + ", Sorting in " + 
                    (desc ? "descending" : "ascending") + " order by: " + sortBy + 
                    " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllByCustomerIdFilteredAndSortedAsync(custId, searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments made " + 
                    (before ? "before" : "after") + " " + when.ToShortDateString() +
                    " by Customer:\t" + name.Name + ", Sorting in " + 
                    (desc ? "descending" : "ascending") + " order by: " + 
                    sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllByCustomerIdFilteredAndSortedAsync
                       (custId, when, before, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching Payments " + 
                    (over ? "over" : "under") + " " + amount.ToString("{0:0.##}") + 
                    " made by Customer:\t" + name.Name + ", Sorting in " + 
                    (desc ? "descending" : "ascending") + " order by: " + sortBy +
                    " and showing Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + custId + ". Details: " + ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return new List<Payment>();
            }
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllByCustomerIdFilteredAndSortedAsync
                       (custId, amount, over, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering Payments by: " + searchBy +
                " and sorting them in " + (desc ? "descending" : "ascending") +
                " order by " + sortBy + ".");

            var pays = await GetAllSortedAsync(sortBy, desc, 
                       await GetAllFilteredAsync(searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (DateTime when, bool before, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo getting Payments " + 
                (before ? "before" : "after") + " " + when.ToShortDateString() +
                " and sorting them in " + (desc ? "descending" : "ascending") +
                " order by " + sortBy + ".");

            var pays = await GetAllSortedAsync(sortBy, desc, 
                       await GetAllFilteredAsync(when, before));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (decimal amount, bool over, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering " + (over ? "over" : "under") +
                amount.ToString("{0:0.##}") + " and sorting them in " + 
                (desc ? "descending" : "ascending") + " order by " + sortBy + ".");

            var pays = await GetAllSortedAsync(sortBy, desc,
                       await GetAllFilteredAsync(amount, over));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
           (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records filtered by: " 
                + searchBy + ", then sorting them in " + (desc ? "descending" : "asending") + 
                " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllFilteredAndSortedAsync(searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async  Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records " + (before ? "before" : "after") +
                " " + when.ToShortDateString() + ", then sorting them in " + (desc ? "descending" : "asending") +
                " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllFilteredAndSortedAsync(when, before, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records " + (over ? "over" : "under") +
                " " + amount.ToString("{0:0.##}") + ", then sorting them in " + (desc ? "descending" : "asending") + 
                " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize);
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllFilteredAndSortedAsync(amount, over, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }


        public async Task<IEnumerable<Payment>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Payment records, sorted by: " + sortBy +
                " in " + (desc ? "descending" : "ascending") + " order, and displaying Page " + pgIndex +
                " of Size: " + pgSize + ".");
            var pays = await GetAllPaginatedAsync(pgSize, pgIndex, 
                       await GetAllSortedAsync(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + pays.Count() + " records.");
            return pays;
        }


        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndPagedAsync(int custId, int pgSize, int pgIndex)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retrieving Payments made by Customer:\t" + custId + "\tNAME: "
                    + (await _context.Customers.FindAsync(custId)).Name + " from Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no payments by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var pays = (await GetAllByCustomerIdAsync(custId)).Skip(pgSize * (pgIndex - 1)).Take(pgSize);
            StaticLogger.LogInfo(GetType(), "Repo found: " + pays.Count() + " Payments on requested page.");
            return pays;
        }

        #endregion

        public async Task<bool> PaymentIdExistsAsync(int payId) => await _context.Payments.FindAsync(payId) != null;

        public async Task<bool> CustomerIdExistsAsync(int custId) => await _context.Customers.FindAsync(custId) != null;

        public async Task<bool> PaymentMethodIdExistsAsync(int methId) => await _context.Customers.FindAsync(methId) != null;

        #endregion
    }
}
