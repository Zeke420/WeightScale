using System.Collections.Generic;
using System.Threading.Tasks;
using WeightScale.DataAccessLayer.Entities;
using WeightScale.DataAccessLayer.Repository;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface ICourierService
    {
        void CreateCourier(string courierName);
        List<Courier> GetCouriers();
        void DeleteCourier(Courier courier);
    }

    public class CourierService : ICourierService
    {
        private readonly ICouriersRepository _couriersRepository;
        
        public CourierService(ICouriersRepository couriersRepository)
        {
            _couriersRepository = couriersRepository;
        }
        
        public void CreateCourier(string courierName)
        {
            var courier = new Courier(courierName);
            _couriersRepository.AddCourier(courier);
        }

        public List<Courier> GetCouriers()
        {
            return _couriersRepository.GetCouriers();
        }

        public void DeleteCourier(Courier courier)
        {
            _couriersRepository.DeleteCourier(courier);
        }
    }
}