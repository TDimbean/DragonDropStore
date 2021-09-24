using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.BLL.DataServices
{
    public class PaymentDataService : IPaymentDataService
    {
        private IPaymentRepository _repo;

        public PaymentDataService(IPaymentRepository repo) => _repo = repo;

        public string Create(Payment pay, bool requiresErrList = false)
        {
            var validation = ValidatePayment(pay);

            if(!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Order failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting record creation for Payment:\t" + pay.PaymentId);
            _repo.Create(pay);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Payment Data Service.");
            return null;
        }

        public string Update(Payment pay, bool requiresErrList = false)
        {
            var idExists = true;
            var idError = "";

            var validation = ValidatePayment(pay);

            if(!_repo.PaymentIdExists(pay.PaymentId))
            {
                idError = "\nNo Payment with ID: " + pay.PaymentId + " found in Repo.\n";
                idExists = false;
            }

            if(!validation.isValid || !idExists)
            {
                StaticLogger.LogError(GetType(), "Payment failed Data Service Validation and will not be sent to Repo for Update. Details:\n" + validation.errorList + idError);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting update for Payment record with ID:\t" + pay.PaymentId);
            _repo.Update(pay);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Payment Data Service.");
            return null;
        }

        #region Gets

        public Payment Get(int id)
        {
            if(!_repo.PaymentIdExists(id))
            {
                StaticLogger.LogWarn(GetType(), "No Payment with ID: " + id + " was found in Repo. Payment Data Service cannot process GET request.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payment with ID:\t" + id + " from Repo.");
            var pay = _repo.Get(id);
            if (pay == null) StaticLogger.LogWarn(GetType(), "Payment Data Service found no Payment with ID:\t" + id);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Payment Data Service.");
            return pay;
        }

        public IEnumerable<Payment> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments from Repo.");
            var pays = _repo.GetAll();
            StaticLogger.LogInfo(GetType(), "Payment Data Service got back: " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllPaginated(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting page " + pgIndex + " of size " + pgSize + " from all Payment records in Repo.");
            var pays = _repo.GetAllPaginated(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        #region Filters

        public IEnumerable<Payment> GetAllFiltered(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payment records from Repo by Search:\t" + searchBy);
            var pays = _repo.GetAllFiltered(searchBy);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments from Repo, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = _repo.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFiltered(DateTime when, bool before)
        {
            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments placed " + befAftText + ":\t" + when.ToShortDateString() + "from Repo.");
            var pays = _repo.GetAllFiltered(when, before);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFiltered(decimal amount, bool over)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments " + (over?"over":"under") + ":\t" + amount + "$ from Repo.");
            var pays = _repo.GetAllFiltered(amount, over);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndPaged(DateTime when, bool before, int pgSize, int pgIndex)
        {
            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting records from Repo, according to \n DATE: " + befAftText + " " + when + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = _repo.GetAllFilteredAndPaged(when, before, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndPaged(decimal amount, bool smaller, int pgSize, int pgIndex)
        {
            var smlGrtText = smaller ? "under" : "over";
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments from Repo, according to \n amount: " + smlGrtText + " than " + amount + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = _repo.GetAllFilteredAndPaged(amount, smaller, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        #endregion

        #region ByCustomerId

        public IEnumerable<Payment> GetAllByCustomerId(int custId)
        {
            if(!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " from Repo.");

            var pays = _repo.GetAllByCustomerId(custId);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Payments cannot proceed.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Servive requesting page: " + pgIndex + " of size: " + pgSize + " from Customer with ID: " + custId + "'s Payments.");
            var pays = _repo.GetAllByCustomerIdAndPaged(custId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got: " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, string searchBy)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " and Filtered by: "+searchBy+" from Repo.");

            var pays = _repo.GetAllByCustomerIdAndFiltered(custId, searchBy);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, DateTime when, bool before)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " " + (before?"before":"after") + " "+ when.ToShortDateString() + " from Repo.");

            var pays = _repo.GetAllByCustomerIdAndFiltered(custId, when, before);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, decimal amount, bool over)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " " + (over?"over":"under") + " " + amount.ToString("0.0:##") + "$ from Repo.");

            var pays = _repo.GetAllByCustomerIdAndFiltered(custId, amount, over);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: "+pgIndex+" of Size: "+pgSize+" of Payments made by Customer with ID:\t" + custId + " and Filtered by: " + searchBy + " from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredAndPaged(custId, searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, DateTime when, bool before, int pgSize, int pgIndex)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" + custId + " " + (before ? "before" : "after") + " " + when.ToShortDateString() + " from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredAndPaged(custId, when, before, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, decimal amount, bool over, int pgSize, int pgIndex)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" + custId + " " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + "$ from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredAndPaged(custId, amount, over, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        #endregion

        #region Sorts

        public  IEnumerable<Payment> GetAllSorted(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting records sorted by: " + sortBy + " in " + (desc?"descending":"ascending") + " order from Repo.");
            var pays = _repo.GetAllSorted(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: "+ pgIndex +" of Size:" + pgSize + " of records sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting records Filtered by: "+searchBy+ " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = _repo.GetAllFilteredAndSorted(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndSorted(DateTime when, bool before, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting payments made " +(before?"before":"after")+ " " + when.ToShortDateString() + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = _repo.GetAllFilteredAndSorted(when, before, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredAndSorted(decimal amount, bool over, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting payments " + (over ? "over" : "under") + " " + amount.ToString("0.0:##")+ "$ and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = _repo.GetAllFilteredAndSorted(amount, over, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdAndSorted(int custId, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " Sorted by: " + sortBy + " in " + (desc?"descending":"ascending") + " order from Repo.");

            var pays = _repo.GetAllByCustomerIdAndSorted(custId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize+ " from records Filtered by: " + searchBy + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = _repo.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredSortedAndPaged(DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of payments made " + (before ? "before" : "after") + " " + when.ToShortDateString() + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = _repo.GetAllFilteredSortedAndPaged(when, before, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllFilteredSortedAndPaged(decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of payments " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + "$ and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = _repo.GetAllFilteredSortedAndPaged(amount, over, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, string searchBy, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " Filtered by: " + searchBy + " and Sorted in "+ (desc?"descending":"ascending") + " order by "+sortBy+" from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredAndSorted(custId, searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, DateTime when, bool before, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " " + (before ? "before" : "after") + " " + when.ToShortDateString() + " and Sorted in " + (desc ? "descending" : "ascending") + " order by " + sortBy + " from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredAndSorted(custId, when, before, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, decimal amount, bool over, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + "$" + " and Sorted in " + (desc ? "descending" : "ascending") + " order by " + sortBy + " from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredAndSorted(custId, amount, over, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdSortedAndPaged(int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: "+pgIndex+" of Size: "+ pgSize+" of Payments made by Customer with ID:\t" + custId + " Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");

            var pays = _repo.GetAllByCustomerIdSortedAndPaged(custId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: "+pgIndex+" of Size: "+pgSize+ " of Payments made by Customer with ID:\t" + custId + " Filtered by: " + searchBy + " and Sorted in " + (desc ? "descending" : "ascending") + " order by " + sortBy + " from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredSortedAndPaged(custId, searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" + custId + " " + (before ? "before" : "after") + " " + when.ToShortDateString() + " and Sorted in " + (desc ? "descending" : "ascending") + " order by " + sortBy + " from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredSortedAndPaged(custId, when, before, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" + custId + " " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + "$" + " and Sorted in " + (desc ? "descending" : "ascending") + " order by " + sortBy + " from Repo.");

            var pays = _repo.GetAllByCustomerIdFilteredSortedAndPaged(custId, amount, over, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        #endregion

        #endregion

        #region Async

        public async Task<string> CreateAsync(Payment pay, bool requiresErrList = false)
        {
            var validation = ValidatePayment(pay);

            if (!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Order failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting record creation for Payment:\t" + pay.PaymentId);
            await _repo.CreateAsync(pay);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Payment Data Service.");
            return null;
        }

        public async Task<string> UpdateAsync(Payment pay, bool requiresErrList = false)
        {
            var idExists = true;
            var idError = "";

            var validation = ValidatePayment(pay);

            if (!await _repo.PaymentIdExistsAsync(pay.PaymentId))
            {
                idError = "\nNo Payment with ID: " + pay.PaymentId + " found in Repo.\n";
                idExists = false;
            }

            if (!validation.isValid || !idExists)
            {
                StaticLogger.LogError(GetType(), 
                    "Payment failed Data Service Validation and will not be sent to Repo for Update."+
                    " Details:\n" + validation.errorList + idError);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting update for Payment record with ID:\t" + pay.PaymentId);
            await _repo.UpdateAsync(pay);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Payment Data Service.");
            return null;
        }

        #region Gets

        public async Task<Payment> GetAsync(int id)
        {
            if (!await _repo.PaymentIdExistsAsync(id))
            {
                StaticLogger.LogWarn(GetType(), "No Payment with ID: " + id + " was found in Repo. Payment Data Service cannot process GET request.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payment with ID:\t" + id + " from Repo.");
            var pay = await _repo.GetAsync(id);
            if (pay == null) StaticLogger.LogWarn(GetType(), "Payment Data Service found no Payment with ID:\t" + id);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Payment Data Service.");
            return pay;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments from Repo.");
            var pays = await _repo.GetAllAsync();
            StaticLogger.LogInfo(GetType(), "Payment Data Service got back: " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllPaginatedAsync(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting page " + pgIndex + " of size " + pgSize + " from all Payment records in Repo.");
            var pays = await _repo.GetAllPaginatedAsync(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        #region Filters

        public async Task<IEnumerable<Payment>> GetAllFilteredAsync(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payment records from Repo by Search:\t" + searchBy);
            var pays = await _repo.GetAllFilteredAsync(searchBy);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments from Repo, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = await _repo.GetAllFilteredAndPagedAsync(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAsync(DateTime when, bool before)
        {
            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments placed " + befAftText + ":\t" + when.ToShortDateString() + "from Repo.");
            var pays = await _repo.GetAllFilteredAsync(when, before);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAsync(decimal amount, bool over)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments " + (over ? "over" : "under") + ":\t" + amount + "$ from Repo.");
            var pays = await _repo.GetAllFilteredAsync(amount, over);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (DateTime when, bool before, int pgSize, int pgIndex)
        {
            var befAftText = before ? "before" : "after";
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting records from Repo, according to \n DATE: " + befAftText + " " + when + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = await _repo.GetAllFilteredAndPagedAsync(when, before, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (decimal amount, bool smaller, int pgSize, int pgIndex)
        {
            var smlGrtText = smaller ? "under" : "over";
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Payments from Repo, according to \n amount: " + smlGrtText + " than " + amount + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var pays = await _repo.GetAllFilteredAndPagedAsync(amount, smaller, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count().ToString() + " records from Repo.");
            return pays;
        }

        #endregion

        #region ByCustomerId

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAsync(int custId)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdAsync(custId);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndPagedAsync
            (int custId, int pgSize, int pgIndex)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Payments cannot proceed.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Servive requesting page: " + pgIndex + " of size: " + pgSize + " from Customer with ID: " + custId + "'s Payments.");
            var pays = await _repo.GetAllByCustomerIdAndPagedAsync(custId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service got: " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, string searchBy)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " and Filtered by: " + searchBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdAndFilteredAsync(custId, searchBy);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, DateTime when, bool before)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " " + (before ? "before" : "after") + " " + when.ToShortDateString() + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdAndFilteredAsync(custId, when, before);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, decimal amount, bool over)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting all Payments made by Customer with ID:\t" + custId + " " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + "$ from Repo.");

            var pays = await _repo.GetAllByCustomerIdAndFilteredAsync(custId, amount, over);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" + custId + " and Filtered by: " + searchBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredAndPagedAsync
                (custId, searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, DateTime when, bool before, int pgSize, int pgIndex)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " + 
                pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" +
                custId + " " + (before ? "before" : "after") + " " + when.ToShortDateString() +
                " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredAndPagedAsync
                (custId, when, before, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, decimal amount, bool over, int pgSize, int pgIndex)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: "
                    + custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" + 
                custId + " " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + "$ from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredAndPagedAsync
                (custId, amount, over, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        #endregion

        #region Sorts

        public async Task<IEnumerable<Payment>> GetAllSortedAsync
            (string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting records sorted by: " +
                sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = await _repo.GetAllSortedAsync(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " +
                pgIndex + " of Size:" + pgSize + " of records sorted by: " + sortBy +
                " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = await _repo.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(),
                "Payment Data Service requesting records Filtered by: " + 
                searchBy + " and Sorted by: " + sortBy + " in " +
                (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = await _repo.GetAllFilteredAndSortedAsync(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + 
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (DateTime when, bool before, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), 
                "Payment Data Service requesting payments made " + 
                (before ? "before" : "after") + " " + when.ToShortDateString() + 
                " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") +
                " order from Repo.");
            var pays = await _repo.GetAllFilteredAndSortedAsync(when, before, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + 
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (decimal amount, bool over, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), 
                "Payment Data Service requesting payments " + 
                (over ? "over" : "under") + " " + amount.ToString("0.0:##") +
                "$ and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = await _repo.GetAllFilteredAndSortedAsync(amount, over, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + 
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdAndSortedAsync
            (int custId, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + 
                    " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(),
                "Payment Data Service requesting all Payments made by Customer with ID:\t" + 
                custId + " Sorted by: " + sortBy + " in " + 
                (desc ? "descending" : "ascending") + " order from Repo.");

            var pays = await _repo.GetAllByCustomerIdAndSortedAsync(custId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " from records Filtered by: " + 
                searchBy + " and Sorted by: " + sortBy + " in " + 
                (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = await _repo.GetAllFilteredSortedAndPagedAsync
                (searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " 
                + pgIndex + " of Size: " + pgSize + " of payments made " +
                (before ? "before" : "after") + " " + when.ToShortDateString() + 
                " and Sorted by: " + sortBy + " in " + 
                (desc ? "descending" : "ascending") + " order from Repo.");
            var pays = await _repo.GetAllFilteredSortedAndPagedAsync
                (when, before, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + 
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " of payments " +
                (over ? "over" : "under") + " " + amount.ToString("0.0:##") +
                "$ and Sorted by: " + sortBy + " in " +
                (desc ? "descending" : "ascending") + " order from Repo.");

            var pays = await _repo.GetAllFilteredSortedAndPagedAsync
                (amount, over, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, string searchBy, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + 
                    " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(),
                "Payment Data Service requesting all Payments made by Customer with ID:\t" +
                custId + " Filtered by: " + searchBy + " and Sorted in " + 
                (desc ? "descending" : "ascending") + " order by " + 
                sortBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredAndSortedAsync
                (custId, searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, DateTime when, bool before, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " +
                    custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(),
                "Payment Data Service requesting all Payments made by Customer with ID:\t" +
                custId + " " + (before ? "before" : "after") + " " + 
                when.ToShortDateString() + " and Sorted in " + 
                (desc ? "descending" : "ascending") + " order by " +
                sortBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredAndSortedAsync
                (custId, when, before, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, decimal amount, bool over, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId +
                    " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(),
                "Payment Data Service requesting all Payments made by Customer with ID:\t" + 
                custId + " " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + 
                "$" + " and Sorted in " + (desc ? "descending" : "ascending") + 
                " order by " + sortBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredAndSortedAsync
                (custId, amount, over, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdSortedAndPagedAsync
            (int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + 
                    custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: "
                + pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" +
                custId + " Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") +
                " order from Repo.");

            var pays = await _repo.GetAllByCustomerIdSortedAndPagedAsync
                (custId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " +
                    custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), 
                "Payment Data Service requesting Page: " + pgIndex + 
                " of Size: " + pgSize + " of Payments made by Customer with ID:\t" +
                custId + " Filtered by: " + searchBy + " and Sorted in " + 
                (desc ? "descending" : "ascending") + " order by " + 
                sortBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredSortedAndPagedAsync
                (custId, searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " +
                    custId + " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Payment Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " of Payments made by Customer with ID:\t" +
                custId + " " + (before ? "before" : "after") + " " + when.ToShortDateString() +
                " and Sorted in " + (desc ? "descending" : "ascending") + 
                " order by " + sortBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredSortedAndPagedAsync
                (custId, when, before, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " +
                pays.Count() + " records from Repo.");
            return pays;
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId +
                    " was found in Repo. Payment Data Service cannot fetch their Payments.");
                return null;
            }

            StaticLogger.LogInfo(GetType(),
                "Payment Data Service requesting Page: " + pgIndex + " of Size: " +
                pgSize + " of Payments made by Customer with ID:\t" + custId +
                " " + (over ? "over" : "under") + " " + amount.ToString("0.0:##") + 
                "$" + " and Sorted in " + (desc ? "descending" : "ascending") + 
                " order by " + sortBy + " from Repo.");

            var pays = await _repo.GetAllByCustomerIdFilteredSortedAndPagedAsync
                (custId, amount, over, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Payment Data Service received " + 
                pays.Count() + " records from Repo.");
            return pays;
        }

        #endregion

        #endregion


        #endregion

        #region Validators

        public (bool isValid, string errorList) ValidatePayment(Payment pay)
        {
            var amtVal = ValidateAmount(pay.Amount);
            var cusVal = ValidateCustomerId(pay.CustomerId);
            var dateVal = ValidateDate(pay.Date);

            var isValid = amtVal.isValid && cusVal.isValid && dateVal.isValid;
            var errorList = new StringBuilder(dateVal.errorList)
                    .AppendLine(cusVal.errorList)
                    .AppendLine(amtVal.errorList);

            if(!_repo.PaymentMethodIdExists(pay.PaymentMethodId))
            {
                errorList.AppendLine("Payment Method not found; Please submit a valid Payment Method ID.");
                isValid = false;
            }

            errorList.Replace("\r\n", "");

            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateAmount(decimal? amt)
        {
            if (amt == null || amt <= 0m) return(false,"Amount must be greater than 0.");
            return (true, string.Empty);
        }

        public (bool isValid, string errorList) ValidateCustomerId(int custId)
        {
            if (!_repo.CustomerIdExists(custId)) return(false, "Payments require a valid CustomerId.");
            return(true, string.Empty);
        }

        public (bool isValid, string errorList) ValidateDate(DateTime? date)
        {
            if (date == null)
                return (false, "A payment's Date cannot be blank.");
            else if (date > DateTime.Now)
                return(false, "A payment cannot occur at a later date; Please review the submitted Payment Date.");
            return (true, string.Empty);
        }

        #endregion
    }
}
