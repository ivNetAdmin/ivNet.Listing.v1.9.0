
using ivNet.Listing.Entities;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using NHibernate;
using Orchard;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using System;
using System.Linq;


namespace ivNet.Listing.Service
{

    public interface IRegistrationServices : IDependency
    {
        void UpdateOwner(RegistrationViewModel viewModel);
        IUser CreateOwnerUser(ActivationViewModel model);
        void UpdateOwnerUserId(string email, int userId);
        RegistrationUpdateViewModel GetOwner(int userId);
    }

    public class RegistrationServices : BaseService, IRegistrationServices
    {
        private readonly IMembershipService _membershipService;
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;

        public RegistrationServices(
            IAuthenticationService authenticationService,
            IRoleService roleService,
            IMembershipService membershipService,
            IRepository<UserRolesPartRecord> userRolesRepository)
            : base(authenticationService)
        {
            _membershipService = membershipService;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public MembershipSettings GetSettings()
        {
            var settings = new MembershipSettings();
            // accepting defaults
            return settings;
        }


        public void UpdateOwner(RegistrationViewModel viewModel)
        {
             using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var ownerKey = CustomStringHelper.BuildKey(new[] { viewModel.Email });

                    // get owner or create a new one
                    var owner = session.CreateCriteria(typeof(Owner))
                        .List<Owner>().FirstOrDefault(
                            x => x.OwnerKey.Equals(ownerKey)) ??
                                   new Owner();

                    if (string.IsNullOrEmpty(ownerKey))
                    {
                        throw new ArgumentException();
                    }

                    owner.Init();
                    
                    MapperHelper.Map(owner, viewModel);
                    owner.AddressDetail = UpdateOwnerAddressDetail(session, viewModel);
                    owner.ContactDetail = UpdateOwnerContactDetail(session, viewModel);

                    owner.IsActive = 1;
                    owner.OwnerKey = ownerKey;

                    SetAudit(owner);
                    session.SaveOrUpdate(owner);

                    transaction.Commit();
                }
             }
        }

        public IUser CreateOwnerUser(ActivationViewModel model)
        {           

            var user = _membershipService.CreateUser(new CreateUserParams(model.Email, model.Password, model.Email, null, null, false));
            var roleRecord = _roleService.GetRoleByName("Owner");

            var existingAssociation =
               _userRolesRepository.Get(record => record.UserId == user.Id && record.Role.Id == roleRecord.Id);
            if (existingAssociation == null)
            {
                _userRolesRepository.Create(new UserRolesPartRecord { Role = roleRecord, UserId = user.Id });
            }

            return user;
        }

        public void UpdateOwnerUserId(string email, int userId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var ownerKey = CustomStringHelper.BuildKey(new[] { email });

                    // get owner or create a new one
                    var owner = session.CreateCriteria(typeof (Owner))
                        .List<Owner>().FirstOrDefault(
                            x => x.OwnerKey.Equals(ownerKey)) ??
                                new Owner();
                    if (string.IsNullOrEmpty(owner.OwnerKey))
                    {
                        throw new ArgumentException();
                    }

                    owner.UserId = userId;
                    SetAudit(owner);
                    session.SaveOrUpdate(owner);

                    transaction.Commit();
                }
            }
        }

        public RegistrationUpdateViewModel GetOwner(int userId)
        {
            var registrationViewModel = new RegistrationUpdateViewModel();

            using (var session = NHibernateHelper.OpenSession())
            {
                var owner = session.CreateCriteria(typeof (Owner))
                    .List<Owner>().FirstOrDefault(
                        x => x.UserId.Equals(userId)) ??
                            new Owner();

                registrationViewModel.UserId = userId;
                registrationViewModel.Surname = owner.Surname;
                registrationViewModel.Firstname = owner.Firstname;
                registrationViewModel.Address1 = owner.AddressDetail.Address1;
                registrationViewModel.Address2 = owner.AddressDetail.Address2;
                registrationViewModel.Town = owner.AddressDetail.Town;
                registrationViewModel.Postcode = owner.AddressDetail.Postcode;
                registrationViewModel.Phone = owner.ContactDetail.Phone;
                registrationViewModel.Email = owner.ContactDetail.Email;
                registrationViewModel.Website = owner.ContactDetail.Website;

            }

            return registrationViewModel;
        }

        private ContactDetail UpdateOwnerContactDetail(ISession session, RegistrationViewModel viewModel)
        {
            var contactDetailKey = CustomStringHelper.BuildKey(new[] {viewModel.Email});

            // get address detail or create a new one
            var contactDetail =  session.CreateCriteria(typeof(ContactDetail))
                .List<ContactDetail>().FirstOrDefault(
                    x => x.ContactDetailKey.Equals(contactDetailKey)) ??
                           new ContactDetail();

            MapperHelper.Map(contactDetail, viewModel);
            contactDetail.ContactDetailKey = contactDetailKey;

            SetAudit(contactDetail);
            session.SaveOrUpdate(contactDetail);
            return contactDetail;
        }

        private AddressDetail UpdateOwnerAddressDetail(ISession session, RegistrationViewModel viewModel)
        {
            var addressDetailKey = CustomStringHelper.BuildKey(new[] {viewModel.Address1, viewModel.Postcode});

            // get address detail or create a new one
            var addressDetail =  session.CreateCriteria(typeof(AddressDetail))
                .List<AddressDetail>().FirstOrDefault(
                    x => x.AddressDetailKey.Equals(addressDetailKey)) ??
                           new AddressDetail();

            MapperHelper.Map(addressDetail, viewModel);
            addressDetail.AddressDetailKey = addressDetailKey;

            SetAudit(addressDetail);
            session.SaveOrUpdate(addressDetail);
            return addressDetail;
        }    
    }
}