using AutoMapper;
using BankCQRS.Application.Contracts.Persistence;
using BankCQRS.Application.Exceptions;
using BankCQRS.Domain.Entities;
using MediatR;
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

        public GetTransactionDetailQueryHandler(IAsyncRepository<Transaction> transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<TransactionDetailVm> Handle(GetTransactionDetailQuery request, CancellationToken cancellationToken)
        {
            var @transaction = await _transactionRepository.GetByIdAsync(request.Id);
            var transactionDetail = _mapper.Map<TransactionDetailVm>(@transaction);

            if (@transaction == null)
            {
                throw new NotFoundException(nameof(Transaction), request.Id);
            }

            return transactionDetail;
        }

    }
}
