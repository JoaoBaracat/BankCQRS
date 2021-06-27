using AutoMapper;
using BankCQRS.Application.Features.Transactions.Commands.CreateTransaction;
using BankCQRS.Application.Features.Transactions.Queries.GetTransactionDetail;
using BankCQRS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionDetailVm>().ReverseMap();
            CreateMap<Transaction, CreateTransactionCommand>().ReverseMap();
        }
    }
}
