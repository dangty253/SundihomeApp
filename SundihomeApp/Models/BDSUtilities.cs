﻿using SundihomeApp.Models;
using SundihomeApp.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace SundihomeApp.Models
{
    public class BDSUtilities
    {
        public static List<Option> GetListUtilities()
        {
            List<Option> list = new List<Option>();
            list.Add(new Option(0, Language.khu_vuc_de_xe_may, "ic_tienichduan_motorpark.png",false));
            list.Add(new Option(1, Language.khu_vuc_de_o_to, "ic_tienichduan_autopark.png", false));
            list.Add(new Option(2, Language.khuon_vien_cay_xanh, "ic_tienichduan_greenarea.png", false));
            list.Add(new Option(3, Language.ghe_cong_cong, "ic_tienichduan_communitychair.png", false));
            list.Add(new Option(4, Language.he_thong_dien_du_phong, "ic_tienichduan_preventiveelectrical.png", false));
            list.Add(new Option(5, Language.he_thong_dieu_hoa_tong, "ic_tienichduan_airconditioningsystem.png", false));
            list.Add(new Option(6, Language.lam_dep_spa, "ic_tienichduan_spa.png", false));
            list.Add(new Option(7, Language.khu_vui_choi_tre_em, "ic_tienichduan_playareaforchildren.png", false));
            list.Add(new Option(8, Language.hop_thu_dan_cu, "ic_tienichduan_residentialmailbox.png", false));
            list.Add(new Option(9, Language.ho_nuoc, "ic_tienichduan_lake.png", false));
            list.Add(new Option(10, Language.khu_tam_linh, "ic_tienichduan_spiritualityarea.png", false));
            list.Add(new Option(11, Language.Smarthome, "ic_tienichduan_smarthome.png", false));
            list.Add(new Option(12, Language.nha_cong_dong, "ic_tienichduan_communityhouse.png", false));
            list.Add(new Option(14, Language.khu_thu_gian_cho_nguoi_cao_tuoi, "ic_tienichduan_relaxationzoneforelderly.png", false));
            list.Add(new Option(15, Language.cong_vien, "ic_tienichduan_park.png", false));
            list.Add(new Option(17, Language.khu_to_chuc_su_kien, "ic_tienichduan_eventzone.png", false));
            list.Add(new Option(18, Language.rap_chieu_phim, "ic_tienichduan_cinema.png", false));
            list.Add(new Option(19, Language.Bar, "ic_tienichduan_bar.png", false));
            list.Add(new Option(20, Language.Karaoke, "ic_tienichduan_karaoke.png", false));
            list.Add(new Option(21, Language.tu_bep, "ic_tienichduan_kitchencabinets.png", false));
            list.Add(new Option(22, Language.bon_tam, "ic_tienichduan_bathtub.png", false));
            list.Add(new Option(23, Language.bon_cau, "ic_tienichduan_toiletbowl.png", false));
            list.Add(new Option(24, Language.bon_rua_mat, "ic_tienichduan_washstand.png", false));
            list.Add(new Option(25, Language.binh_nong_lanh, "ic_tienichduan_storagewaterheater.png", false));
            list.Add(new Option(26, Language.may_hut_bui, "ic_tienichduan_hood.png", false));
            list.Add(new Option(27, Language.ban_an, "ic_tienichduan_diningtable.png", false));
            list.Add(new Option(28, Language.ke_tivi, "ic_tienichduan_tvshelf.png", false));
            list.Add(new Option(29, Language.tu_trang_tri, "ic_tienichduan_decorativecabinet.png", false));
            list.Add(new Option(30, Language.giuong_ngu, "ic_tienichduan_bed.png", false));
            list.Add(new Option(31, Language.ban_phan, "ic_tienichduan_makeuptable.png", false));
            list.Add(new Option(32, Language.tu_quan_ao, "ic_tienichduan_wardrobe.png", false));
            list.Add(new Option(33, Language.voi_hoa_sen, "ic_tienichduan_shower.png", false));
            list.Add(new Option(34, Language.bo_bien, "ic_tienichduan_beach.png", false));
            list.Add(new Option(35, Language.phong_xong_hoi, "ic_tienichduan_sauna.png", false));
            list.Add(new Option(36, Language.sofa_phong_khach, "ic_tienichduan_livingroomsofa.png", false));
            list.Add(new Option(37, Language.ban_tra, "ic_tienichduan_teatable.png", false));
            list.Add(new Option(38, Language.dem, "ic_tienichduan_mattress.png", false));
            list.Add(new Option(39, Language.tran_tha, "ic_tienichduan_dropceiling.png", false));
            list.Add(new Option(40, Language.dieu_hoa, "ic_tienichduan_airconditioner.png", false));
            list.Add(new Option(41, Language.Internet, "ic_tienichduan_internet.png", false));
            list.Add(new Option(42, Language.thiet_bi_bao_chay, "ic_tienichduan_firesystem.png", false));
            list.Add(new Option(43, Language.truyen_hinh_cap, "ic_tienichduan_cabletv.png", false));
            list.Add(new Option(44, Language.bep_gas_dien, "ic_tienichduan_gasstove.png", false));
            list.Add(new Option(45, Language.vuon_treo_tren_cao, "ic_tienichduan_hanginggarden.png", false));
            list.Add(new Option(46, Language.xe_bus, "ic_tienichduan_busservice.png", false));
            list.Add(new Option(47, Language.cong_vien_nuoc, "ic_tienichduan_waterpark.png", false));
            list.Add(new Option(48, Language.he_thong_gas_tong, "ic_tienichduan_overallgassystem.png", false));
            list.Add(new Option(49, Language.dai_phun_nuoc, "ic_tienichduan_fountain.png", false));
            list.Add(new Option(50, Language.quang_truong, "ic_tienichduan_square.png", false));
            list.Add(new Option(51, Language.ben_du_thuyen, "ic_tienichduan_marinas.png", false));
            list.Add(new Option(52, Language.thu_vien, "ic_tienichduan_library.png", false));
            list.Add(new Option(53, Language.Casino, "ic_tienichduan_casino.png", false));
            list.Add(new Option(54, Language.tu_am_tuong, "ic_tienichduan_wallcabinet.png", false));
            list.Add(new Option(55, Language.tu_lanh, "ic_tienichduan_fridge.png", false));
            list.Add(new Option(56, Language.san_truot_bang, "ic_tienichduan_skatingrink.png", false));
            list.Add(new Option(57, Language.Tivi, "ic_tienichduan_tv.png", false));
            list.Add(new Option(58, Language.may_giat, "ic_tienichduan_washingmachine.png", false));
            list.Add(new Option(59, Language.lo_vi_song, "ic_tienichduan_microwave.png", false));
            list.Add(new Option(60, Language.may_rua_bat, "ic_tienichduan_dishwasher.png", false));
            list.Add(new Option(61, Language.may_phat_dien, "ic_tienichduan_electricmachine.png", false));
            list.Add(new Option(62, Language.dieu_hoa_am_tran, "ic_tienichduan_conditionerceiling.png", false));
            list.Add(new Option(63, Language.he_thong_chong_trom, "ic_tienichduan_antitheftsystem.png", false));
            list.Add(new Option(64, Language.may_say_quan_ao, "ic_tienichduan_dryer.png", false));
            list.Add(new Option(65, Language.dai_quan_sat, "ic_tienichduan_observatory.png", false));
            list.Add(new Option(66, Language.ban_lam_viec, "ic_tienichduan_workingdesh.png", false));
            list.Add(new Option(67, Language.tu_giay, "ic_tienichduan_shoescabinet.png", false));
            list.Add(new Option(68, Language.may_chieu, "ic_tienichduan_projector.png", false));
            list.Add(new Option(69, Language.may_in, "ic_tienichduan_printer.png", false));
            list.Add(new Option(70, Language.tu_tai_lieu, "ic_tienichduan_documentcabinet.png", false));
            list.Add(new Option(71, Language.tap_dau_giuong, "ic_tienichduan_bedtab.png", false));
            list.Add(new Option(72, Language.le_tan, "ic_tienichduan_reception.png", false));
            list.Add(new Option(73, Language.an_ninh_bao_ve, "ic_tienichduan_security.png", false));
            list.Add(new Option(74, Language.don_ve_sinh, "ic_tienichduan_cleaningservice.png", false));
            list.Add(new Option(75, Language.the_dan_cu, "ic_tienichduan_membercard.png", false));
            list.Add(new Option(76, Language.camera_giam_sat, "ic_tienichduan_securitycamera.png", false));
            list.Add(new Option(77, Language.nha_hang, "ic_tienichduan_restaurant.png", false));
            list.Add(new Option(78, Language.sieu_thi, "ic_tienichduan_supersmrket.png", false));
            list.Add(new Option(79, Language.coffee_shop, "ic_tienichduan_coffee.png", false));
            list.Add(new Option(80, Language.trung_tam_thuong_mai, "ic_tienichduan_businesscenter.png", false));
            list.Add(new Option(81, Language.ATM, "ic_tienichduan_atm.png", false));
            list.Add(new Option(82, Language.be_boi_ngoai_troi, "ic_tienichduan_swimmingpooloutside.png", false));
            list.Add(new Option(83, Language.be_boi_4_mua, "ic_tienichduan_swimmingpool4seasons.png", false));
            list.Add(new Option(84, Language.phong_cho, "ic_tienichduan_lounge.png", false));
            list.Add(new Option(85, Language.truong_mam_non, "ic_tienichduan_preschool.png", false));
            list.Add(new Option(86, Language.truong_hoc_cap_1_2_3, "ic_tienichduan_highschool.png", false));
            list.Add(new Option(87, Language.benh_vien_phong_kham, "ic_tienichduan_healthfacility.png", false));
            list.Add(new Option(88, Language.shop_thoi_trang, "ic_tienichduan_fashionshop.png", false));
            list.Add(new Option(89, Language.hieu_giat_la, "ic_tienichduan_laundry.png", false));
            list.Add(new Option(90, Language.be_boi_trong_nha, "ic_tienichduan_swimmingpoolInhouse.png", false));
            list.Add(new Option(91, Language.san_tennis, "ic_tienichduan_tenniscourt.png", false));
            list.Add(new Option(92, Language.san_cau_long, "ic_tienichduan_badmintoncourt.png", false));
            list.Add(new Option(93, Language.fitness, "ic_tienichduan_fitness.png", false));
            list.Add(new Option(94, Language.ngan_hang, "ic_tienichduan_bank.png", false));
            list.Add(new Option(95, Language.BBQ, "ic_tienichduan_bbq.png", false));
            list.Add(new Option(96, Language.san_tap_golf, "ic_tienichduan_golfpractice.png", false));
            list.Add(new Option(97, Language.san_golf, "ic_tienichduan_golfcourt.png", false));
            list.Add(new Option(98, Language.nha_thuoc, "ic_tienichduan_drugstore.png", false));
            list.Add(new Option(99, Language.cua_hang_hoa_tuoi, "ic_tienichduan_flowershop.png", false));
            list.Add(new Option(100, Language.cho, "ic_tienichduan_market.png", false));
            list.Add(new Option(101, Language.san_da_bong_mini, "ic_tienichduan_minifootballcourt.png", false));
            list.Add(new Option(102, Language.khu_vuc_choi_bong_ban, "ic_tienichduan_areafortabletennis.png", false));
            list.Add(new Option(103, Language.khu_vuc_choi_bi_a, "ic_tienichduan_areaforbilliard.png", false));
            list.Add(new Option(104, Language.san_bong_ro, "ic_tienichduan_volleyballcourt.png", false));
            return list;
        } 
    }
}
