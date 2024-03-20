using domain;

namespace sqlAndWebApi.Services
{
    public class CostumerService
    {
        private readonly ITestService _testService;
        public CostumerService(ITestService testservice)
        {
            _testService = testservice;   
        }

        public bool RegirsterUser(Registration registration,Person person)
        {
            if(_testService.GetPersonByUsername(registration.userName) != null)
            {
                return false;
            }
            _testService.AddUser(person);
            return true;
           
        }
        public bool GetAllParticipants()
        {
            if(_testService.GetAll()==null)
            {
                return false;
            }
            return true;
        }
        //public bool GetPersonById(int id)
        //{
        //    if(_testService.Find(id)!= null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        public bool DeleteUser(int id)
        {
            if(_testService.Find(id)!=null)
            {
                return true;
            }
            return false;
        }
        public bool UpdateUser(int id)
        {
            if(_testService.Find(id)==null)
            {
                return false;
            }
            return true;
        }
    }
}
