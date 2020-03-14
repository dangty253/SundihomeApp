using System;
namespace SundihomeApp.Configuration
{
    public class ApiRouter
    {
        public const string DELETE_IMAGE = ApiConfig.CloudStorageApi + "/api/files/delete";

        // USER API
        public const string USER_AVATAR_UPDATE = "api/user/changeavatar";
        public const string USER_AVATAR_UPLOAD = ApiConfig.CloudStorageApi + "/api/files/upload?folder=avatar";
        public const string USER_GET_USER_BY_ID = "api/user";
        public const string USER_PROFILE_UPDATE = "api/user/updateprofile";
        public const string USER_SOCIALLOGIN = "api/auth/sociallogin";
        public const string USER_LOGIN = "api/auth/login";
        public const string USER_CHECKUSER = "api/auth/checkuser";
        public const string USER_CHECKPHONE = "api/auth/checkphone";
        public const string USER_LOGGOUT = "api/user/logout";
        public const string USER_CREATE = "api/auth/create";
        public const string USER_CHANGEEMAIL = "api/user/changeemail";

        // POST
        public const string POST_GETBYPROJECT = "api/post/GetPostInCurrentProject";
        public const string POST_GETBYID = "api/post";
        public const string POST_CREATE = "api/post";
        public const string POST_UPDATE = "api/post/update";
        public const string POST_DELETE = "api/post";
        public const string POST_COMMENT = "api/post/comment";
        public const string POST_GETCOMMENT = "api/post/comment";
        public const string POST_GETBYALLCOMPANIES = "api/post/byallcompanies";
        public const string POST_GETBYCOMPANY = "api/post/bycompany"; // /CompanyId


        // FURNITURE CATEGORY
        public const string FURNITURECATEGORY_GET_ONLY_PARENT = "api/furniturecategory/onlyparent";
        public const string FURNITURECATEGORY_GET_BY_PARENT = "api/furniturecategory/byparent";
        public const string FURNITURECATEGOR_GET_CATEGORY = "api/furniturecategory";



        // FURNITURE PRODUCT
        public const string FURNITUREPRODUCT_FILTER = "api/furnitureproduct/filter";
        public const string FURNITUREPRODUCT_GET_PRODUCT_COMPANY = "api/furnitureproduct/company";
        public const string FURNITUREPRODUCT_MY_PRODUCT = "api/furnitureproduct/myproducts";
        public const string FURNITUREPRODUCT_ADD_UPDATE = "api/furnitureproduct";
        public const string FURNITUREPRODUCT_REMOVE = "api/furnitureproduct";
        public const string FURNITUREPRODUCT_IMAGE_UPLOAD = ApiConfig.CloudStorageApi + "/api/files/upload?folder=furniture/product";
        public const string FURNITUREPRODUCT_GET_BY_ID = "api/furnitureproduct?productid=";
        public const string FURNITUREPRODUCT_GETNEW = "api/furnitureproduct/new";
        public const string FURNITUREPRODUCT_SLIDEITEME = "api/furnitureproduct/slideitems";
        public const string FURNITUREPRODUCT_ADVERTISE = "api/furnitureproduct/GetAdvertises";
        public const string FURNITUREPRODUCT_UPDATE_PRODUCT_STATUS = "api/furnitureproduct/product_status";
        public const string FURNITUREPRODUCT_GETNEWPROMOTIONS = "api/furnitureproduct/new_promotions";

        // POST

        //FURNITUER POST ITEM
        public const string FURNITUREPOSTITEM_GETALL = "api/furniturepostitem";

        // PROJECT
        public const string PROJECT_GET_BYCOMPANYID = "api/project/company";

        //  PROJECT DIARY
        public const string PROJECT_DIARY_GET_LIST_PROJECTDIARY_BYPROJECTID = "api/project/dairies";
        public const string PROJECT_DIARY_DELETE_PROEJCTDIARY = "api/project/dairy";
        public const string PROJECT_DIARY_ADD_PROJECTDIARY = " api/project/dairy";
        public const string PROJECT_DIARY_GET_ONE_PROJECTDIARY = "api/project/dairy";
        public const string PROJECT_DIARY_UPDATE = "api/project/dairy";

        // COMPNAY
        public const string COMANY_GETBYID = "api/company";
        public const string COMPANY_ADD = "api/company";
        public const string COMPANY_UPDATE = "api/company";
        public const string COMPANY_GET_INVITE_USER = "api/company/GetInviteUserList";
        public const string COMPANY_GET_EMPLOYEE = "api/company/GetUser";
        public const string COMPANY_GET_LICHSUPHATTRIENCONGTY = "api/company/list_lichsuphattrien";
        public const string COMPANY_POST_LICHSUPHATTRIENCONGTY = "api/company/add_lichsuphattrien";
        public const string COMPANY_DELETE_LICHSUPHATTRIENCONGTY = "api/company/remove_lichsuphattrien";
        public const string COMPANY_GET_THANHTUUCONGTY = "api/company/list_thanhtuu";
        public const string COMPANY_POST_THANHTUUCONGTY = "api/company/add_thanhtuu";
        public const string COMPANY_DELETE_THANHTUUCONGTY = "api/company/remove_thanhtuu";
        public const string COMPANY_GET_NHANVIENUUTUCONGTY = "api/company/GetListNhanVienUuTu";
        public const string COMPANY_GETPOSTLIST = "api/company/posts";
        public const string COMPANY_APPROVE_POST = "api/company/approvepost"; // put
        public const string COMPANY_REMOVOUT_POST = "api/company/removepost"; // delete
        public const string COMPANY_REJECT_POST = "api/company/rejectpost"; // put
        public const string COMPANY_ADDTO_GIOCHUNG = "api/company/addtogiochung"; // put
        public const string COMPANY_GETNEWPOST = "api/company/new_posts"; // /{id}
        public const string COMPANY_GETNEWCONCTACTNEEDS = "api/company/new_contactneeds"; // /{id}
        public const string COMPANY_GET_CONTACT = "api/contact/filter";
        public const string COMPANY_GET_ALLCONTACTNEEDS = "api/contact/allcontactneeds";
        public const string COMPANY_GET_FILTER_CONTACTNEEDS = "api/contact/filter_contactneeds";



        // MOI GIOI
        public const string MOIGIOI_REGISTER = "api/moigioi/register";
        public const string MOIGIOI_GETALL = "api/moigioi";
        public const string MOIGIOI_GETBYID = "api/moigioi";
        public const string MOIGIOI_SHAREPOST = "api/moigioi/sharepost";

        //TASK
        public const string TASK_CRUD = "api/congviec";
        public const string TASK_GETMYTASKS = "api/congviec/mytasks";
        public const string TASK_UPDATE_COMPLETED = "api/congviec/completed";
        public const string TASK_NEWTASKS = "api/congviec/mynewtasks";

        // EMPLOYEE
        public const string EMPLOYEE_REGISTER = "api/employee/register/";
        public const string EMPLOYEE_APPROVE_HOSODANGKY = "api/employee/approve_hosodangky";
        public const string EMPLOYEE_REJECT_HOSODANGKY = "api/employee/reject_hosodangky";
        public const string EMPLOYEE_GETMYPOSTLIST = "api/employee/myposts"; // iscommitment,page
        public const string EMPLOYEE_CONFIRM_COMPANNY = "api/employee/confirmcompany";
        public const string EMPLOYEE_GET_EMPLOYEE_BY_PHONE = "api/employee/GetUserByPhone";
        public const string EMPLOYEE_GETNEWPOSTS = "api/employee/newposts";
        public const string EMPLOYEE_GETNEWCONTACTNEEDS = "api/employee/newcontactneeds";

        //CONTACT
        public const string CONTACT_GETMYCONTACTS = "api/contact/mycontacts";
        public const string CONTACT_ADD = "api/contact";
        public const string CONTACT_PUT = "api/contact";
        public const string CONTACT_DELETE = "api/contact";
        public const string CONTACT_GET_NEEDS = "api/contact/get_needs";
        public const string CONTACT_ADD_NEED = "api/contact/add_need";
        public const string CONTACT_DELETE_NEED = "api/contact/delete_need";
        public const string CONTACT_PUT_NEED = "api/contact/update_need";
        public const string CONTACT_GET_POSTCARES = "api/contact/postcares";//ContactId
        public const string CONTACT_POST_POSTCARES = "api/contact/postcare";
        public const string CONTACT_DELETE_POSTCARES = "api/contact/postcare";//ContactId,PostId

        // LIQUIDATION
        public const string LIQUIDATION_GETBYTYPE = "api/liquidation/new";
        public const string LIQUIDATION_FILTER = "api/liquidation/filter";
        public const string LIQUIDATION_SAVE = "api/liquidation";
        public const string LIQUIDATION_GETBYID = "api/liquidation"; // vi du api/liquidation/3196F0A4-17F7-4A85-90E0-59C60C1E5DCB
        public const string LIQUIDATION_DELETE = "api/liquidation";
        public const string LIQUIDATION_SLIDEITEM = "api/liquidation/slideitems";
        public const string LIQUIDATION_ADVERTISE = "api/liquidation/GetAdvertises";

        // LIQUIDATION TODAY
        public const string LIQUIDATIONTODAY_GETTODAY = "api/liquidationtoday";
        public const string LIQUIDATIONTODAY_SAVE = "api/liquidationtoday";
        public const string LIQUIDATIONTODAY_FILTER = "api/liquidationtoday/filter";
        public const string LIQUIDATIONTODAY_DELETE = "api/liquidationtoday";
        public const string LIQUIDATIONTODAY_CHECKTIME = "api/liquidationtoday/checktime";
        public const string LIQUIDATIONTODAY_REPOST = "api/liquidationtoday/repost";

        // LIQUIDATION POST ITEM
        public const string LIQUIDATIONPOSTITEM_GETALL = "api/liquidationpostitem";

        // B2BPOST ITEM
        public const string B2BPOSTITEM_LIST = "api/b2bpostitem";

        // INTERNAL POST ITEM
        public const string INTERNAL_POSTITEMLIST = "api/internalpostitem";

        // BANK
        public const string BANK_GETALL = "api/bank";
        public const string BANK_EMPLOYEE_REGISTER = "api/bank/employee/register";
        public const string BANK_EMPLOYEE_UPDATE = "api/bank/employee";
        public const string BANK_EMPLOYEE_CHECK = "api/bank/employee/check"; // kiem tra co phai nhan vien khong
        public const string BANK_EMPLOYEE_DETAIL = "api/bank/employee/";
        public const string BANK_GOIVAY = "api/bank/goivay";

        // GIA DAT
        public const string GIADAT_STREET = "api/giadat/streets"; // /{DistrictId}
        public const string GIADAT_STREETDISTANCE = "api/giadat/street_distances"; // /{StreetId}
        public const string GIADAT_HANOI_KHUDOTHI = "api/giadat/hanoi/khudothi"; // /{DistrictId}
        public const string GIADAT_HANOI_STREETS = "api/giadat/hanoi/streets"; // /{DistrictId}
        public const string GIADAT_GIADATHANOI_MATCATDUONG = "api/giadat/hanoi/matcatduong"; // /{KhuDoThiId}
        public const string GIADAT_GIADATHANOI_STREET_DISTANCES = "api/giadat/hanoi/street_distances"; // /{KhuDoThiId}
        public const string GIADAT_STREET_WARD = "api/giadat/streets/ward"; // /{WardId}

        public const string GIADAT_BINHDUONG_STREET = "api/giadat/binhduong/streets";//{DistrictId}
        public const string GIADAT_BINHDUONG_STREET_DISTANCES = "api/giadat/binhduong/street_distances";//{StreetId}

        public const string GIADAT_NAMDINH_STREETS_DISTRICT = "api/giadat/namdinh/streets/district";//{DistrictId}
        public const string GIADAT_NAMDINH_STREETS_WARD = "api/giadat/namdinh/streets/ward";//{WardId}
        public const string GIADAT_NAMDINH_STREET_DISTANCES = "api/giadat/namdinh/street_distances";//{StreetId}

        public const string GIADAT_DONGNAI_STREETS_DISTRICT = "api/giadat/dongnai/streets/district";//{DistrictId}
        public const string GIADAT_DONGNAI_STREETS_WARD = "api/giadat/dongnai/streets/ward";//{WardId}
        public const string GIADAT_DONGNAI_STREET_DISTANCES = "api/giadat/dongnai/street_distances";//{StreetId}

        public const string GIADAT_BACNINH_STREETS_DISTRICT = "api/giadat/bacninh/streets/district";//{DistrictId}"
        public const string GIADAT_BACNINH_STREETS_WARD = "api/giadat/bacninh/streets/ward";//{WardId}"
        public const string GIADAT_BACNINH_KHUDANCU_DISTRICT = "api/giadat/bacninh/khudancu/district";//{DistrictId}"
        public const string GIADAT_BACNINH_KHUDANCU_WARD = "api/giadat/bacninh/khudancu/ward";//{WardId}"
        public const string GIADAT_BACNINH_KHUVUC_WARD = "api/giadat/bacninh/khuvuc/ward";//{WardId}"
        public const string GIADAT_BACNINH_STREETS_DISTANCES = "api/giadat/bacninh/street_distances";//{StreetId}"
        public const string GIADAT_BACNINH_KHUDANCU = "api/giadat/bacninh/khudancu";//{KhuDanCuId}"

        public const string GIADAT_YENBAI_STREETS = "api/giadat/yenbai/streets";//{DistrictId}
        public const string GIADAT_YENBAI_STREET_DISTANCES = "api/giadat/yenbai/street_distances";//{StreetId}

        public const string GIADAT_SONLA_STREETS = "api/giadat/sonla/streets";//{DistrictId}
        public const string GIADAT_SONLA_STREET_DISTANCES = "api/giadat/sonla/street_distances";//{StreetId}
    }

}
