﻿using System.Collections.Generic;
using System.Threading.Tasks;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.DataAccessLayer.Repository
{
    public interface ICouriersRepository
    {
        void AddCourier(Courier courier);
        void UpdateCourier(Courier courier);
        void DeleteCourier(Courier courier);
        List<Courier> GetCouriers();
    }
}