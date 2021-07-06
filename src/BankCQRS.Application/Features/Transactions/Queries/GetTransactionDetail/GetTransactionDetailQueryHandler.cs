using AutoMapper;
using BankCQRS.Application.Contracts.Persistence;
using BankCQRS.Application.Exceptions;
using BankCQRS.Application.Models.Cache;
using BankCQRS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankCQRS.Application.Features.Transactions.Queries.GetTransactionDetail
{
    public class GetTransactionDetailQueryHandler : IRequestHandler<GetTransactionDetailQuery, TransactionDetailVm>
    {
        private readonly IAsyncRepository<Transaction> _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private const string TRANSACTION_ID = "Id";
        private readonly MemoryCacheSettings _memoryCacheSettings;

        public GetTransactionDetailQueryHandler(IAsyncRepository<Transaction> transactionRepository, IMapper mapper, IMemoryCache memoryCache, IOptions<MemoryCacheSettings> option)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _memoryCacheSettings = option.Value;
        }

        public async Task<TransactionDetailVm> Handle(GetTransactionDetailQuery request, CancellationToken cancellationToken)
        {

            if (_memoryCache.TryGetValue(TRANSACTION_ID, out object transactionObject))
            {
                return (TransactionDetailVm)transactionObject;
            }
            else
            {
                var @transaction = await _transactionRepository.GetByIdAsync(request.Id);
                var transactionDetail = _mapper.Map<TransactionDetailVm>(@transaction);

                if (@transaction == null)
                {
                    throw new NotFoundException(nameof(Transaction), request.Id);
                }

                _memoryCache.Set(TRANSACTION_ID, transactionDetail, new CacheEntryOptionsFactory(_memoryCacheSettings).CacheEntryOptions());

                return transactionDetail;
            }
        }

    }
}
