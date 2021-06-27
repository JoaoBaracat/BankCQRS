using BankCQRS.Application.Contracts.Persistence;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Features.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        private readonly ITransactionRepository _transactionRepository;

        public CreateTransactionCommandValidator(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;

            RuleFor(x => x.AccountOrigin)
                .NotEmpty().WithMessage("The {PropertyName} must be supplied");

            RuleFor(x => x.AccountDestination)
                .NotEmpty().WithMessage("The {PropertyName} must be supplied");

            RuleFor(x => x).Must(x => x.AccountDestination != x.AccountOrigin).WithMessage("The accounts must be different");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("The {PropertyName} must be supplied")
                .GreaterThan(0).WithMessage("The {PropertyName} must be greater than 0");
        }
    }
}
