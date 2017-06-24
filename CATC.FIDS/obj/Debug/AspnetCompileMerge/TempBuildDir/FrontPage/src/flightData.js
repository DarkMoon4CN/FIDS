export default (function flightData($, util, layer, layui, doT) {
    //四个弹框
    let $planBoxMask = null, //航班计划弹框
        $resBoxMask  = null, //资源分配弹框
        $baseBoxMask = null;
    //iscroll实例->代表水平和垂直实例
    let scrollCount  = null, scrollHCount = null;
    
    //layui's form初始化
    let form = layui.form();

    let getElem = function(){
        $planBoxMask = $('.js-modal-fliData .js-flight-plan'), //航班计划弹框
        $resBoxMask  = $('.js-modal-fliData .js-resource-allo'), //资源分配弹框
        $baseBoxMask = $('.js-modal-fliData .js-base-data');//基础数据弹框
    }
    
    //公共方法
    let pub = {
        setModalCon: function () {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        },
        //绑定飞机数据
        bindPlaneData: function(){
            const $planeDataList = $('#plane-data').children('.is-active');
            const $planeDataChilds = $planeDataList.children();
            const $planeData = $baseBoxMask.find('.plane-data');
            //id
            let id = $planeDataList.attr('data-id');
            $planeData.find('.js-flag-id').val(id);
            //飞机注册号
            let reg = $planeDataChilds.map(function(index, item){
                return item.getAttribute('data-reg-no');
            })[0];
            $planeData.find('input[name="ac_reg_no"]').val(reg);
            //机型IATA代码
            let type_iata = $planeDataChilds.map(function(index, item){
                return item.getAttribute('data-type-iata');
            })[0];
            $planeData.find('input[name="ac_type_iata"]').val(type_iata);
            //航空公司IATA代码
            let airline_iata = $planeDataChilds.map(function(index, item){
                return item.getAttribute('data-airline-iata');
            })[0];
            $planeData.find('input[name="airline_iata"]').val(airline_iata);
            //删除标志
            let flg_del = $planeDataChilds.map(function(index, item){
                return item.getAttribute('data-flg-del');
            })[0];
            $planeData.find('select[name="flag_del"]').find('option[value="'+ flg_del +'"]').prop('selected','selected');
            form.render();
            //附加代码
            let ext_code = $planeDataChilds.map(function(index, item){
                return item.getAttribute('data-ext-code');
            })[0];
            $planeData.find('input[name="ext_code"]').val(ext_code);
        },
        //机型数据
        bindModelData: function(){
            const $curList = $('#model-data').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.model-data');
            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-model-id').val(id);
            //中文名称
            let name_cn = $curChilds.map(function(index, item){
                return item.getAttribute('data-name-cn');
            })[0];
            $modalData.find('input[name="cn_name"]').val(name_cn);
            //英文名称
            let name_en = $curChilds.map(function(index, item){
                return item.getAttribute('data-name-en');
            })[0];
            $modalData.find('input[name="en_name"]').val(name_en);
            //机型IATA代码
            let iata_code = $curChilds.map(function(index, item){
                return item.getAttribute('data-iata-code');
            })[0];
            $modalData.find('input[name="ac_type_iata"]').val(iata_code);
            //机型IACO代码
            let icao_code = $curChilds.map(function(index, item){
                return item.getAttribute('data-icao-code');
            })[0];
            $modalData.find('input[name="ac_type_iaco"]').val(icao_code);
        },
        //航空公司
        bindAirlineData: function(){
            const $curList = $('#airline-data').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.airline');

            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-airline-id').val(id);
            //简称
            let short_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国内国际属性
            let dori = $curChilds.map(function(index, item){
                return item.getAttribute('data-dori');
            })[0];
            $modalData.find('select[name="dori"] option[value="'+dori+'"]').prop('selected','selected');
            form.render();
            //航空公司IATA代码
            let airline_iata = $curChilds.map(function(index, item){
                return item.getAttribute('data-airline-iata');
            })[0];
            $modalData.find('input[name="airline_iata"]').val(airline_iata);
            //航空公司IACO代码
            let airline_icao = $curChilds.map(function(index, item){
                return item.getAttribute('data-airline-icao');
            })[0];
            $modalData.find('input[name="airline_icao"]').val(airline_icao);
            //基地机场IACO代码
            let airport_iata = $curChilds.map(function(index, item){
                return item.getAttribute('data-airport-iata');
            })[0];
            $modalData.find('input[name="airport_iata"]').val(airport_iata);
            //航空公司联盟代码
            let alliance = $curChilds.map(function(index, item){
                return item.getAttribute('data-alliance-code');
            })[0];
            $modalData.find('input[name="alliance_code"]').val(alliance);
        },
        //航空联盟
        bindAllianceData: function(){
            const $curList = $('#air-alliance').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.air-alliance');

            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-alliance-id').val(id);

            //联盟代码
            let all_code = $curChilds.map(function(index, item){
                return item.getAttribute('data-alliance-name');
            })[0];
            $modalData.find('input[name="alliance_code"]').val(all_code);
            //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //备注
            let remark = $curChilds.map(function(index, item){
                return item.getAttribute('data-remark');
            })[0];
            $modalData.find('input[name="remark"]').val(remark);
        },
        //机场
        bindAirportData: function(){
            const $curList = $('#airport-data').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.airport');

            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-airport-id').val(id);

            //简称
            let short_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国内国际属性
            let dori = $curChilds.map(function(index, item){
                return item.getAttribute('data-dori');
            })[0];
            $modalData.find('select[name="dori"] option[value="'+ dori +'"]').prop('selected','selected');
            form.render();
            //机场IATA代码
            let airport_iata = $curChilds.map(function(index, item){
                return item.getAttribute('data-airport-iata');
            })[0];
            $modalData.find('input[name="airport_iata"]').val(airport_iata);
            //机场ICAO代码
            let airport_icao = $curChilds.map(function(index, item){
                return item.getAttribute('data-airport-icao');
            })[0];
            $modalData.find('input[name="airport_icao"]').val(airport_icao);
            //城市IATA代码
            let city_iata = $curChilds.map(function(index, item){
                return item.getAttribute('data-city-iata');
            })[0];
            $modalData.find('input[name="city_iata"]').val(city_iata);
            //区域代码
            let area_code = $curChilds.map(function(index, item){
                return item.getAttribute('data-area-code');
            })[0];
            $modalData.find('input[name="area_code"]').val(area_code);
        },
        //城市
        bindCityData: function(){
            const $curList = $('#city-data').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.city');
            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-city-id').val(id);
            //简称
            let short_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //所属省份标志
            let province_flag = $curChilds.map(function(index, item){
                return item.getAttribute('data-province');
            })[0];
            $modalData.find('input[name="province_flag"]').val(province_flag);
            //城市IATA代码
            let city_iata = $curChilds.map(function(index, item){
                return item.getAttribute('data-city-iata');
            })[0];
            $modalData.find('input[name="city_iata"]').val(city_iata);
            //城市ICAO代码
            let city_icao = $curChilds.map(function(index, item){
                return item.getAttribute('data-city-icao');
            })[0];
            $modalData.find('input[name="city_icao"]').val(city_icao);
            //国际IATA代码
            let country_iata = $curChilds.map(function(index, item){
                return item.getAttribute('data-country-iata');
            })[0];
            $modalData.find('input[name="con_iata"]').val(country_iata);
        },
        //省份
        bindProvinceData: function(){
            const $curList = $('#province-data').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.province');
            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-province-id').val(id);
            //简称
            let short_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国内国际属性
            let dori = $curChilds.map(function(index, item){
                return item.getAttribute('data-dori');
            })[0];
            $modalData.find('select[name="dori"] option[value="'+ dori +'"]').prop('selected','selected');
            form.render();
        },
        //国家
        bindCountryData: function(){
            const $curList = $('#country-data').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.country');
            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-country-id').val(id);
             //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国家IATA代码
            let country_iata = $curChilds.map(function(index, item){
                return item.getAttribute('data-country-iata');
            })[0];
            $modalData.find('input[name="country_iata"]').val(country_iata);
            //国家ICAO代码
            let country_icao = $curChilds.map(function(index, item){
                return item.getAttribute('data-country-icao');
            })[0];
            $modalData.find('input[name="country_icao"]').val(country_icao);
        },
        //任务代码
        bindTaskCodeData: function(){
            const $curList = $('#task-code').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.task-code');
            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-task-id').val(id);
             //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //任务代码
            let task_code = $curChilds.map(function(index, item){
                return item.getAttribute('data-task-code');
            })[0];
            $modalData.find('input[name="task_code"]').val(task_code);
            //描述
            let desc = $curChilds.map(function(index, item){
                return item.getAttribute('data-desc');
            })[0];
            $modalData.find('input[name="desc"]').val(desc);
        },
        //延误代码
        bindDelayCodeData: function(){
            const $curList = $('#delay-code').children('.is-active');
            const $curChilds = $curList.children();
            const $modalData = $baseBoxMask.find('.delay-code');
            //id
            let id = $curList.attr('data-id');
            $modalData.find('.js-delay-id').val(id);
             //中文名称
            let cn_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            let en_name = $curChilds.map(function(index, item){
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //延误代码
            let delay_code = $curChilds.map(function(index, item){
                return item.getAttribute('data-delay-code');
            })[0];
            $modalData.find('input[name="delay_code"]').val(delay_code);
            //延误类型
            let delay_type = $curChilds.map(function(index, item){
                return item.getAttribute('data-delay-type');
            })[0];
            $modalData.find('input[name="delay_type"]').val(delay_type);
            //描述
            let desc = $curChilds.map(function(index, item){
                return item.getAttribute('data-desc');
            })[0];
            $modalData.find('input[name="desc"]').val(desc);
        },
        /**离港航班->绑定单条数据到弹框
         * @param id Number类型 单条数据的FDID
         */
        bindDeparFlightData: function(id){
            util.sendPostReq('/FlightDataApi/GetFlightData',{FDID: id}).then(function(data){
                if(data.Status){
                    bindData(data.Data);
                } else {
                    layer.msg(data.Message);
                }
            }).fail(function(){
                layer.msg('获取单条数据失败');
            });

            function bindData(data) {
                //FDID
                $planBoxMask.find('.js-fdid').val(id);
                //运营日
                if(data.OPERATION_DATE){
                    let oper_day = new Date(Number(/(\d+)/.exec(data.OPERATION_DATE)[1])).toISOString().formatTime('{0}-{1}-{2}');
                    $('#oper-day-dep').val(oper_day);
                }          
                //航班号
                $('#flight-num-dep').val(data.FLIGHT_NO);
                //国内国际标识
                $('#dori-dep').find('option[value="'+ data.DORI+'"]').prop('selected',"selected");
                //任务代码
                $('#task-code-d').find('option[value="'+ data.TASK_CODE+'"]').prop('selected',"selected");
                //航站楼
                $('#term-floor-dep').val(data.TERMINAL_NO);
                //航空公司
                $('#airline-dep').val(data.AIRLINE_IATA);
                //机号
                $('#plane-num-dep').val(data.AC_REG_NO);
                //机型
                $('#plane-type-dep').val(data.AIRCRAFT_TYPE_IATA);
                //贵客标识
                $('#flag-vip-dep').find('option[value="'+ data.FLG_VIP+'"]').prop('selected',"selected");
                //本场起飞
                $('#take-off-dep').val(data.ORIGIN_AIRPORT_IATA);
                //后场到达
                $('#arrive-dep').val(data.DEST_AIRPORT_IATA);
                //计划本场离港时间
                if(data.STD){
                    let std = new Date(Number(/(\d+)/.exec(data.STD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#std-dep').val(std);
                }
                
                //计划后场到港时间
                if(data.STA){
                    let sta = new Date(Number(/(\d+)/.exec(data.STA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#sta-dep').val(sta);
                }
                
                //预计离港时间
                if(data.ETD){
                    let etd = new Date(Number(/(\d+)/.exec(data.ETD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#etd-dep').val(etd);
                }
                //预计到岗时间
                if(data.ETA){
                    let eta = new Date(Number(/(\d+)/.exec(data.ETA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#eta-dep').val(eta);
                }
                //实际离港时间
                if(data.ATD){
                    let atd = new Date(Number(/(\d+)/.exec(data.ATD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#atd-dep').val(atd);
                }
                //实际到港时间
                if(data.ATA){
                    let ata = new Date(Number(/(\d+)/.exec(data.ATA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#ata-dep').val(ata);
                }
                
                //航线起始
                $('#airline-start-dep').val(data.AIRPORT1);
                //航显结束
                $('#airline-end-dep').val(data.AIRPORT4);
                //经停1
                $('#airline-1-dep').val(data.AIRPORT2);
                //经停2
                $('#airline-2-dep').val(data.AIRPORT3);
                //共享主航班号
                $('#share-main-dep').val(data.CODE_SHARE1);
                //共享航班号
                $('#share-2-dep').val(data.CODE_SHARE2);
                //链接航班
                //$('#link-flight').val();
                //链接运营日
                //$('#link-day').val();
                //合并航班号
                $('#merge-dep').val(data.CODE_SHARE3);
                //被合并航班号
                $('#be-merge-dep').val(data.CODE_SHARE4);

                form.render();
            }
            
        },
        /**到港航班->绑定单条数据到弹框
         * @param id Number类型 单条数据的FDID
         */
        bindArvFlightData: function(id){
            util.sendPostReq('/FlightDataApi/GetFlightData',{FDID: id}).then(function(data){
                if(data.Status){
                    bindData(data.Data);
                } else {
                    layer.msg(data.Message);
                }
            }).fail(function(){
                layer.msg('获取单条数据失败');
            });
            function bindData(data){
                 //FDID
                $planBoxMask.find('.js-fdid').val(id);
                //运营日
                if(data.OPERATION_DATE){
                    let oper_day = new Date(Number(/(\d+)/.exec(data.OPERATION_DATE)[1])).toISOString().formatTime('{0}-{1}-{2}');
                    $('#oper-day-arr').val(oper_day);
                }          
                //航班号
                $('#flight-num-arr').val(data.FLIGHT_NO);
                //国内国际标识
                $('#dori-arr').find('option[value="'+ data.DORI+'"]').prop('selected',"selected");
                //任务代码
                $('#task-code-a').find('option[value="'+ data.TASK_CODE+'"]').prop('selected',"selected");
                //航站楼
                $('#term-floor-arr').val(data.TERMINAL_NO);
                //航空公司
                $('#airline-arr').val(data.AIRLINE_IATA);
                //机号
                $('#plane-num-arr').val(data.AC_REG_NO);
                //机型
                $('#plane-type-arr').val(data.AIRCRAFT_TYPE_IATA);
                //贵客标识
                $('#flag-vip-arr').find('option[value="'+ data.FLG_VIP+'"]').prop('selected',"selected");
                //本场起飞
                $('#take-off-arr').val(data.ORIGIN_AIRPORT_IATA);
                //后场到达
                $('#arrive-arr').val(data.DEST_AIRPORT_IATA);
                //计划本场离港时间
                if(data.STD){
                    let std = new Date(Number(/(\d+)/.exec(data.STD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#std-arr').val(std);
                }
                
                //计划后场到港时间
                if(data.STA){
                    let sta = new Date(Number(/(\d+)/.exec(data.STA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#sta-arr').val(sta);
                }
                
                //预计离港时间
                if(data.ETD){
                    let etd = new Date(Number(/(\d+)/.exec(data.ETD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#etd-arr').val(etd);
                }
                //预计到岗时间
                if(data.ETA){
                    let eta = new Date(Number(/(\d+)/.exec(data.ETA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#eta-arr').val(eta);
                }
                //实际离港时间
                if(data.ATD){
                    let atd = new Date(Number(/(\d+)/.exec(data.ATD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#atd-arr').val(atd);
                }
                //实际到港时间
                if(data.ATA){
                    let ata = new Date(Number(/(\d+)/.exec(data.ATA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#ata-arr').val(ata);
                }
                
                //航线起始
                $('#airline-start-arr').val(data.AIRPORT1);
                //航显结束
                $('#airline-end-arr').val(data.AIRPORT4);
                //经停1
                $('#airline-1-arr').val(data.AIRPORT2);
                //经停2
                $('#airline-2-arr').val(data.AIRPORT3);
                //共享主航班号
                $('#share-main-arr').val(data.CODE_SHARE1);
                //共享航班号
                $('#share-2-arr').val(data.CODE_SHARE2);
                //链接航班
                //$('#link-flight').val();
                //链接运营日
                //$('#link-day').val();
                //合并航班号
                $('#merge-arr').val(data.CODE_SHARE3);
                //被合并航班号
                $('#be-merge-arr').val(data.CODE_SHARE4);

                form.render();
            }
            
        },
        /** 分页 
         *  @param option Object类型
         *  option.pageNum Number类型 总页数
         *  option.url String类型 请求链接
         *  option.pageSize Number类型 一页显示多少条
         *  option.tmplId  String类型 模版ID
         *  option.targetId String类型 目标ID
         */
        laypageEvent: function (option) {                      
            layui.use(['laypage'], function () {
                let laypage = layui.laypage;        
                laypage({
                    cont: 'log-view-paging',
                    pages: option.pageNum,
                    groups: 4,
                    skin: '#e6e6e6',
                    last: '尾页',
                    prev: '<em><i class="icon-prev"></i></em>',
                    next: '<em><i class="icon-next"></i></em>',
                    jump: function (obj, first) {
                        if (!first) {
                            let loadTips = layer.load(2, {
                                shade: 0.02
                            });
                            //根据页数obj.curr发送对应的请求
                            util.sendPostReq(option.url, {
                                    startRow: (obj.curr - 1) * option.pageSize,
                                    pageSize: option.pageSize,
                                })
                                .then(function (data) {
                                    if (data.Status) {
                                        let res = doT.template($('#'+option.tmplId).html())(data.Data.List);
                                        $('#'+option.targetId).html(res);
                                    } else {
                                        layer.msg(data.Message);
                                    }
                                })
                                .then(function(){
                                    //内容列表添加局部滚动
                                    pub.removeScrollbar();
                                    scrollHCount = util.myHScroll('.js-tabCtrl-con');
                                    scrollCount = util.myScroll('.js-tableCon');
                                })
                                .always(function () {
                                    layer.close(loadTips);
                                });
                        }
                    }
                });

            });
        },
        //切换弹框类型
        changeModalType: function (attr) {
            switch (attr) {
                //航班计划弹框
                case 'flight-plan':
                    $planBoxMask.show();
                    break;
                    //资源分配弹框
                case 'res-allo':
                    $resBoxMask.show();
                    break;
                    //基础数据弹框
                case 'base-data':
                    $baseBoxMask.show();
                    break;
            }
        },

        /** 航班数据->改变弹框内容
         * @param attr String类型 根据自定义属性的值来切换不同的弹框内容
         * @param id String类型 点击编辑按钮，给对应的弹框加上对应的行id
         */
        changeModalCon: function (attr, id) {
            id = id || '';
            switch (attr) {
                //航班计划
                case 'depart':
                    $planBoxMask.find('.depart-form').addClass('show').siblings('.arrive-form').removeClass('show').addClass('hide');
                    //如果id存在，则是编辑->获取单条航班数据
                    id ? pub.bindDeparFlightData(id) : null;          
                    break;
                case 'arrive':
                    $planBoxMask.find('.arrive-form').addClass('show').siblings('.depart-form').removeClass('show').addClass('hide');
                    id ? pub.bindArvFlightData(id) : null;
                    break;
                    //资源分配
                case 'board-gate':
                    $resBoxMask.find('.board-gate-res').addClass('show').siblings('.board-check-bag').removeClass('show').addClass('hide');
                    break;
                case 'check-in':
                    $resBoxMask.find('.check-in-res').addClass('show').siblings('.board-check-bag').removeClass('show').addClass('hide');
                    break;
                case 'baggage':
                    $resBoxMask.find('.baggage-res').addClass('show').siblings('.board-check-bag').removeClass('show').addClass('hide');
                    break;
                    //基础数据
                case 'plane-data':
                    $baseBoxMask.find('.plane-data').addClass('show').siblings().removeClass('show').addClass('hide');
                    //id有值则是编辑，把选中行的数据绑定到弹框
                    id ? pub.bindPlaneData() : null;
                    break;
                case 'model-data':
                    $baseBoxMask.find('.model-data').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindModelData() : null;
                    break;
                case 'airline':
                    $baseBoxMask.find('.airline').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindAirlineData() : null;
                    break;
                case 'air-alliance':
                    $baseBoxMask.find('.air-alliance').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindAllianceData() : null;
                    break;
                case 'airport':
                    $baseBoxMask.find('.airport').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindAirportData() : null;
                    break;
                case 'city':
                    $baseBoxMask.find('.city').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindCityData() : null;
                    break;
                case 'province':
                    $baseBoxMask.find('.province').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindProvinceData() : null;
                    break;
                case 'country':
                    $baseBoxMask.find('.country').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindCountryData() : null;
                    break;
                case 'task-code':
                    $baseBoxMask.find('.task-code').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindTaskCodeData() : null;
                    break;
                case 'delay-code':
                    $baseBoxMask.find('.delay-code').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindDelayCodeData() : null;
                    break;
            }
        },

        //删除横向竖向滚动条
        removeScrollbar: function () {
            $('.iScrollHorizontalScrollbar.iScrollLoneScrollbar').remove();
            $('.iScrollVerticalScrollbar.iScrollLoneScrollbar').remove();
        },
        //次日航班、资源分配等切换一级切换
        dataTabsChangeCon: function (elem) {
            let $changeMenuCon = $('.js-changeMenuCon'),
                $menuDisplay = $('.js-menuDisplay'),
                curAttr = $(elem).attr('data-flag');

            switch (curAttr) {
                case 'morrow-plan':
                    $menuDisplay.removeClass('menuHide');
                    $changeMenuCon.html('发布数据');
                    //获取初始数据
                    pub.tableConDataInit({
                        reqData: {
                            aord: 'D',
                            planType: 0
                        },
                        tmplId: 'flight-plan-tmpl',
                        elemId: 'flight-plan-d'
                    });
                    break;
                case 'today-plan':
                    $changeMenuCon.html('次日数据转当日数据');
                    $menuDisplay.addClass('menuHide');
                    pub.tableConDataInit({
                        reqData: {
                            aord: 'D',
                            planType: 1
                        },
                        tmplId: 'flight-plan-tmpl',
                        elemId: 'flight-plan-td'
                    });
                    break;
                case 'morrow-res':
                    $changeMenuCon.html('发布数据');
                    $menuDisplay.addClass('menuHide');
                    break;
                case 'today-res':
                    $changeMenuCon.html('次日资源转当日资源');
                    $menuDisplay.addClass('menuHide');
                    break;
                case 'base-data':
                    $changeMenuCon.html('发布数据');
                    $menuDisplay.addClass('menuHide');
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetAirCraftData',
                         tmplId: 'plane-data-tmpl',
                         elemId: 'plane-data',
                    });
                    break;
            }
        },
        //离港航班、进港航班、登机口资源等二级切换
        ctrlTabChangeCon: function (elem) {
            let curAttr = $(elem).attr('data-menu');
            //一级切换元素的flag属性
            let curFlag = $('.js-dataTabs').children().eq(0).children('.is-active').attr('data-flag');
            switch (curAttr) {
                case 'depart':
                    if (curFlag === 'morrow-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'D',
                                planType: 0
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-d'
                        });
                    }
                    if (curFlag === 'today-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'D',
                                planType: 1
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-td'
                        });
                    }
                    break;
                case 'arrive':
                    if (curFlag === 'morrow-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'A',
                                planType: 0
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-a'
                        });

                    }
                    if (curFlag === 'today-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'A',
                                planType: 1
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-ta'
                        });
                    }
                    break;
                case 'plane-data':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetAirCraftData',
                         tmplId: 'plane-data-tmpl',
                         elemId: 'plane-data'
                    });
                    break;
                case 'model-data':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetAirCraftType',
                         tmplId: 'model-data-tmpl',
                         elemId: 'model-data'
                    }); 
                    break;
                case 'airline':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetAirLineData',
                         tmplId: 'airline-data-tmpl',
                         elemId: 'airline-data'
                    }); 
                    break;
                case 'air-alliance':
                    pub.tableConDataInit({
                            url: '/FlightDataApi/GetAllianceData',
                            tmplId: 'air-alliance-tmpl',
                            elemId: 'air-alliance'
                    }); 
                    break;
                case 'airport':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetAirportData',
                         tmplId: 'airport-data-tmpl',
                         elemId: 'airport-data'
                    }); 
                    break;
                case 'city':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetCity',
                         tmplId: 'city-data-tmpl',
                         elemId: 'city-data'
                    }); 
                    break;
                case 'province':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetProvince',
                         tmplId: 'province-data-tmpl',
                         elemId: 'province-data'
                    }); 
                    break;
                case 'country':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetCountry',
                         tmplId: 'country-data-tmpl',
                         elemId: 'country-data'
                    }); 
                    break;
                case 'task-code':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetTaskCode',
                         tmplId: 'task-code-tmpl',
                         elemId: 'task-code'
                    }); 
                    break;
                case 'delay-code':
                    pub.tableConDataInit({
                         url: '/FlightDataApi/GetDelayCode',
                         tmplId: 'delay-code-tmpl',
                         elemId: 'delay-code'
                    }); 
                    break;
            }

        },
        sendFlightDataFormVal: function(){
            form.on('submit(flight-data)', function(data){
                let formObj = data.field;
                let obj = {};
                //如果fdid有值，则是编辑，不是添加
                if(formObj.fdid){
                    obj.FDID = ~~formObj.fdid;
                }
                //运营日
                obj.OPERATION_DATE = formObj.oper_day;
                //航班号
                obj.FLIGHT_NO = formObj.flight_num;
                //到离港标识
                if($(this).closest('.layui-form').hasClass('depart-form')){
                    obj.AORD = 'D';
                }
                if($(this).closest('.layui-form').hasClass('arrive-form')){
                    obj.AORD = 'A';
                }
                //国内国际标识
                obj.DORI= formObj.dori;
                //任务代码
                obj.TASK_CODE = formObj.task_code;
                //航站楼
                obj.TERMINAL_NO = ~~formObj.term_floor;
                //航空公司
                obj.AIRLINE_IATA = formObj.airline;
                //机号
                obj.AC_REG_NO = formObj.plane_num;
                //机型
                obj.AIRCRAFT_TYPE_IATA = formObj.plane_type;
                //贵宾标识
                obj.FLG_VIP = formObj.flg_vip;
                //起飞
                obj.ORIGIN_AIRPORT_IATA = formObj.take_off;
                //到达
                obj.DEST_AIRPORT_IATA = formObj.arrive;
                //计划离港
                obj.STD = formObj.std;
                //预计离港
                obj.ETD = formObj.etd;
                //实际离港
                obj.ATD = formObj.atd;
                //计划到港
                obj.STA = formObj.sta;
                //预计到港
                obj.ETA = formObj.eta;
                //实际到港
                obj.ATA = formObj.ata;
                //航显起始
                obj.AIRPORT1 = formObj.airline_start;
                //航显结束
                obj.AIRPORT4 = formObj.airline_end;
                //经停1
                obj.AIRPORT2 = formObj.air_line_1;
                //经停2
                obj.AIRPORT3 = formObj.air_line_2;
                //共享主航班号
                obj.CODE_SHARE1 = formObj.share_main;
                //共享航班号
                obj.CODE_SHARE2 = formObj.share_code2;
                //合并航班号
                 obj.CODE_SHARE3 = formObj.merge_flight;
                //被合并航班号
                 obj.CODE_SHARE4 = formObj.be_merge_fli;

                
                
                console.log('formObj',formObj);
                console.log('obj',obj);
                
            })

        },
        //基础数据弹框->发送表单信息
        sendBaseDataFormVal: function () {
            //submit提交
            form.on('submit(base-data)', function (data) {
                var formObj = data.field;
                //飞机数据
                if($(this).closest('.layui-form').hasClass('plane-data')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.AC_REG_NO = formObj.ac_reg_no;
                    obj.AC_TYPE_IATA = formObj.ac_type_iata;
                    obj.AIRLINE_IATA = formObj.airline_iata;
                    obj.FLG_DELETED = formObj.flag_del;
                    obj.EXT_CODE = formObj.ext_code;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairCraftData', obj)
                            .then(function (data) { 
                                console.log('1',data);
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetAirCraftData',
                                        tmplId: 'plane-data-tmpl',
                                        elemId: 'plane-data',
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }
                //机型数据
                if($(this).closest('.layui-form').hasClass('model-data')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.name_chinese = formObj.cn_name;
                    obj.name_english = formObj.en_name;
                    obj.iataCode = formObj.ac_type_iata;
                    obj.icaoCode = formObj.ac_type_iaco; 
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairCraftType', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetAirCraftType',
                                        tmplId: 'model-data-tmpl',
                                        elemId: 'model-data'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }
                //航空公司
                if($(this).closest('.layui-form').hasClass('airline')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.Airline_IATA = formObj.airline_iata;
                    obj.Airline_ICAO  = formObj.airline_icao;
                    obj.Short_Name = formObj.short_name;
                    obj.Host_AirPort_IATA  = formObj.airport_iata; 
                    obj.DORI  = formObj.dori; 
                    obj.NAME_ENGLISH  = formObj.en_name; 
                    obj.NAME_CHINESE  = formObj.cn_name; 
                    obj.ALLIANCE_CODE  = formObj.alliance_code;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairLineData', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetAllianceData',
                                        tmplId: 'airline-data-tmpl',
                                        elemId: 'airline-data'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }
                //航空联盟
                if($(this).closest('.layui-form').hasClass('air-alliance')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.ALLIANCE_NAME = formObj.alliance_code;
                    obj.NAME_CHINESE = formObj.cn_name;
                    obj.NAME_ENGLISH = formObj.en_name;
                    obj.REMARK = formObj.remark;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddallianceData', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetAllianceData',
                                        tmplId: 'air-alliance-tmpl',
                                        elemId: 'air-alliance'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }
                //机场
                if($(this).closest('.layui-form').hasClass('airport')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.AIRPORT_IATA = formObj.airport_iata;
                    obj.AIRPORT_ICAO  = formObj.airport_icao;
                    obj.SHORT_NAME = formObj.short_name;
                    obj.CITY_IATA  = formObj.city_iata; 
                    obj.DORI  = formObj.dori; 
                    obj.NAME_ENGLISH = formObj.en_name; 
                    obj.NAME_CHINESE = formObj.cn_name; 
                    obj.REGION_CODE = formObj.area_code;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairPortData', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetAirportData',
                                        tmplId: 'airport-data-tmpl',
                                        elemId: 'airport-data'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }
                //城市
                if($(this).closest('.layui-form').hasClass('city')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.City_IATA = formObj.city_iata;
                    obj.Country_IATA = formObj.con_iata;
                    obj.City_ICAO = formObj.city_icao;
                    obj.Name_Chinese = formObj.cn_name; 
                    obj.Name_English = formObj.en_name; 
                    obj.Short_Chinese = formObj.short_name; 
                    obj.Province_IS = formObj.province_flag ? ~~formObj.province_flag : formObj.province_flag;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        console.log(obj);
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddCityData', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetCity',
                                        tmplId: 'city-data-tmpl',
                                        elemId: 'city-data'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }      
                //省份
                if($(this).closest('.layui-form').hasClass('province')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.Province_ID = ~~formObj.id;
                    }
                    obj.Short_Name = formObj.short_name;
                    obj.Name_Chinese = formObj.cn_name;
                    obj.Name_English = formObj.en_name;
                    obj.DORI = formObj.dori;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddProvinceData', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetProvince',
                                        tmplId: 'province-data-tmpl',
                                        elemId: 'province-data'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }
                //国家
                if($(this).closest('.layui-form').hasClass('country')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.Country_IATA = formObj.country_iata;
                    obj.Name_Chinese = formObj.cn_name;
                    obj.Name_English = formObj.en_name;
                    obj.Country_ICAO = formObj.country_icao;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddCountryData', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetCountry',
                                        tmplId: 'country-data-tmpl',
                                        elemId: 'country-data'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }
                //任务代码
                if($(this).closest('.layui-form').hasClass('task-code')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.Task_Code = formObj.task_code;
                    obj.Name_Chinese = formObj.cn_name;
                    obj.Name_English = formObj.en_name;
                    obj.Description = formObj.desc;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddTaskCode', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetFlightBasicsData',
                                        tmplId: 'task-code-tmpl',
                                        elemId: 'task-code'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                } 
                //延误代码
                if($(this).closest('.layui-form').hasClass('delay-code')){
                    let obj = {};
                    //如果id有值，则是编辑，不是添加
                    if(formObj.id){
                       obj.ID = ~~formObj.id;
                    }
                    obj.Delay_Code = formObj.delay_code;
                    obj.Type = formObj.delay_type;
                    obj.Code_Chinese = formObj.cn_name;
                    obj.Code_English = formObj.en_name;
                    obj.Description = formObj.desc;
                    // 发送数据前检测obj是否为空
                    let isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        let loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        console.log(obj);
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddDelayCode', obj)
                            .then(function (data) { 
                                if (data.Status) {
                                    layer.msg(data.Message);
                                    $baseBoxMask.hide();
                                    $('.js-reset').trigger('click');
                                    pub.tableConDataInit({
                                        url: '/FlightDataApi/GetDelayCode',
                                        tmplId: 'delay-code-tmpl',
                                        elemId: 'delay-code'
                                    });
                                } else {
                                    layer.msg(data.Message);
                                }
                            })
                            .fail(function (err) {
                                layer.msg('发送请求失败');
                            })
                            .always(function () {
                                layer.close(loadTips);
                            });
                    }
                }           
                return false;
            });
        },
        /** 航班数据列表初始化
         * @param option Object类型
         * option.url: 请求链接
         * option.reqDate: 请求主体
         * option.tmplId: doT模版ID
         * option.elemId: 目标ID
         */
        tableConDataInit: function (option) {
            option.reqData = option.reqData || {};
            option.url = option.url || '/FlightDataApi/GetFlightDataList';
            option.reqData.pageSize = 12;
            //如果选中了全选按钮，每次初始化应该去掉选中效果
            $('.js-allEquip').hasClass('checkbox-checked') ? $('.js-allEquip').removeClass('checkbox-checked') : null;
            
            //请求列表
            let loadTips = layer.load(2, {
                shade: 0.03
            });
            util.sendPostReq(option.url, option.reqData)
                .then(function (data) {
                    if (data.Status) {  
                        if (data.Data.List.length > 0) {
                            let str = $('#' + option.tmplId).html();
                            let res = doT.template(str)(data.Data.List);
                            $('#' + option.elemId).html(res);
                            //总数据count->作为第二个then的参数
                            return data.Data.Count;
                        } else {
                            $('#' + option.elemId).html('');
                        }
                    } else {
                        layer.msg(data.Message);
                    }
                })
                .then(function(count){

                    //内容列表添加局部滚动
                    pub.removeScrollbar();
                    scrollHCount = util.myHScroll('.js-tabCtrl-con');
                    scrollCount = util.myScroll('.js-tableCon');
                    //分页
                    pub.laypageEvent({
                        pageNum: Math.ceil(count/option.reqData.pageSize),
                        url: option.url,
                        pageSize: option.reqData.pageSize,
                        tmplId: option.tmplId,
                        targetId: option.elemId
                    });
                })
                .fail(function (err) {
                    layer.msg('请求发生错误');
                })
                .always(function () {
                    layer.close(loadTips);
                });
        }
    };
    //航班数据表tableCon中的事件
    let flightDataTableEvent = function () {

        //tabCtrl-> 切换到/离港航班
        $('.js-tabCtrl').on('click', 'div', function (e) {
            let $dataTable = $(this).closest('.js-dataTable');
            //不同选项发送不同请求 
            pub.ctrlTabChangeCon(this);
            //TAB切换清除选中效果
            $dataTable.find('.row.is-active').removeClass('is-active');
            $dataTable.find('.checkbox.checkbox-checked').removeClass('checkbox-checked');
            e.stopPropagation();
        });

        let $tableCon = $('.js-dataTable .js-tableCon');

        //tableCon列表点击效果
        $tableCon.on('click', function (e) {
            var tar = e.target,
                $parTag = $(tar.parentNode),
                flag = $parTag.hasClass('row');
            //由于iscoll点击一次会触发两次，所以先销毁，在函数尾部再添加上
            /* scrollCount ? util.destroyScroll(scrollCount) : null;
             scrollHCount ? util.destroyScroll(scrollHCount) : null;*/
            if (flag) {
                //点击当前row时->移除全部row复选框的效果
                $(this).find('.checkbox').removeClass('checkbox-checked');
                $('.allEquip').removeClass('checkbox-checked');
                //点击时背景改变
                if ($parTag.attr('switch')) {
                    $parTag.removeAttr('switch');
                    $parTag.removeClass('is-active');
                    $parTag.find('.checkbox').removeClass('checkbox-checked');
                } else {
                    $parTag.attr('switch', 'on').siblings().removeAttr('switch');
                    $parTag.addClass('is-active').siblings().removeClass('is-active');
                    $parTag.find('.checkbox').addClass('checkbox-checked');
                }
            }
            /*//添加上iscroll实例
            scrollHCount = util.myHScroll('.js-tabCtrl-con');
            scrollCount = util.myScroll('.js-tableCon');*/
        });
        //列表的每个复选框 & row背景色改变
        $tableCon.on('click', '.checkbox', function (e) {
            $('.js-allEquip').removeClass('checkbox-checked');
            let $this = $(this),
                $row = $this.closest('.row');
            let flag = $this.hasClass('checkbox-checked');
            if (!flag) {
                $this.addClass('checkbox-checked');
                $row.addClass('is-active');
            } else {
                $this.removeClass('checkbox-checked');
                $row.removeClass('is-active');
                // $('.js-allEquip').removeClass('checkbox-checked');
            }
            e.stopPropagation();
        });
        //全选按钮
        $('.js-allEquip').on('click', function (e) {
            var tar = e.target,
                gar = tar.parentNode.parentNode,
                $this = $(this);
            var $tableCon = $(gar).siblings('.js-tableCon');
            var $checkboxs = $tableCon.find('.checkbox');
            var $rows = $tableCon.find('.row');
            var flag = $this.hasClass('checkbox-checked');
            if (!flag) {
                //$this-> 控制的是当前按钮的开关效果
                $this.addClass('checkbox-checked');
                $rows.addClass('is-active');
                $checkboxs.addClass('checkbox-checked');
                $rows.removeAttr('switch');
            } else {
                $this.removeClass('checkbox-checked');
                $checkboxs.removeClass('checkbox-checked');
                $rows.removeClass('is-active');
            }

        });
    };

    //弹框中的事件
    let modalEvent = function () {
        var $resetBtn = $('.js-modal-fliData .js-reset');
        //确认按钮
        $('.js-modal-fliData .ensure-btn').on('click', function () {
            let $this = $(this);
            let $boxMask = $this.closest('.mod-boxMask');
    
            //点击上传
            let $gar = $this.parent().parent();
            if ($gar.hasClass('importFile')) {
                $.ajaxFileUpload({
                    url: '/FlightDataApi/UploadFile',
                    fileElementId: 'path',
                    secureuri: false,
                    dataType: 'json',
                    success: function (data) {
                        console.log('data', data);
                    },
                    error: function (err) {
                        console.log('err', err);
                    }
                });
            }
            // $boxMask.hide();
            // $resetBtn.trigger('click');
        });
        //取消
        $('.js-modal-fliData .cancel-btn').on('click', function () {
            let $boxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $boxMask.hide();
        });
        //关闭按钮
        $('.js-modal-fliData .close-btn').on('click', function () {
            let $boxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $boxMask.hide();
        });

        //上传按钮
        $(".upload").on("change", "input[type='file']", function (e) {
            var filePath = $(this).val();
            if (filePath.indexOf("xlsx") != -1 || filePath.indexOf("xls") != -1) {
                $(".input-tips").html("").hide();
                var arr = filePath.split('\\');
                var fileName = arr[arr.length - 1];
                $(".show-file-name").val(fileName);
            } else {
                $(".show-file-name").val("");
                $(".input-tips").html("您未上传文件，或者您上传文件类型有误！").show();
                return false;
            }
        });

    };

    //菜单menu中的事件
    let menuEvent = function () {
        let $title = $('.js-modal-fliData .js-modalBigTitle');
        let $importTitle = $('.js-import-data .js-modalBigTitle');
        //添加数据
        $('.js-menu-add').on('click', function () {
            let cur = $('.js-dataTable.show .js-tabCtrl').find('.is-click');
            let curText = cur.text();
            $title.html('添加' + curText);
            //根据不同的列表类型改变弹框部分内容
            let tabAttr = cur.attr('data-menu');
            pub.changeModalCon(tabAttr);
            //根据dataTabs的不同显示对应的弹框
            let curAttr = $('.js-dataTabs .dataTabs').find('.is-active').attr('data-modal');
            pub.changeModalType(curAttr);
            
        });
        //编辑数据
        $('.js-menu-edit').on('click', function () {
            let checkLength = $('.js-tableCon').find('.checkbox.checkbox-checked').length;     
            if (checkLength === 0) {
                layer.alert('未选中数据');
            } else if (checkLength > 1) {
                layer.alert('只能编辑单条数据');
            } else {
                //选中行的id
                let dataId = $('.js-tableCon').find('.row.is-active').attr('data-id');
                //当前二级切换的元素
                let $curEle = $('.js-dataTable.show .js-tabCtrl').find('.is-click');
                let curText = $curEle.text();
                $title.html('编辑' + curText);
                //根据不同的列表类型改变弹框部分内容
                let tabAttr = $curEle.attr('data-menu');
                pub.changeModalCon(tabAttr, dataId);
                //根据dataTabs的不同显示对应的弹框
                let curAttr = $('.js-dataTabs .dataTabs').find('.is-active').attr('data-modal');
                pub.changeModalType(curAttr);
                
            }

        });
        //导入数据
        $('.js-menu-save').on('click', function () {
            $importTitle.html('上传文件');
            $('.js-import-data').show();
        });
        //发布数据
        $('.js-menu-send').on('click', function () {

        });
        //删除按钮
        $('.js-menu-del').on('click', function (e) {

            //获取列表选中行
            let $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            let checkLength = $tableCon.length;
            let rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            let ids = rows.map(function (row) {
                return row.getAttribute('data-rowid')
            }).join(',');
            if (checkLength === 0) {
                layer.alert('未选中数据');
            } else {
                // pub.delData(ids);
            }
        });
    };

    //多种数据切换事件:次日数据、资源分配、基础...
    let dataTabsEvent = function () {

        $('.js-dataTabs .dataTabs').on('click', 'li', function (e) {
            //根据航班数据、资源分配...不同改变列表内容
            pub.dataTabsChangeCon(this);

            //数据切换->清除选中行和全选btn样式
            var $dataTabs = $(this).closest('.js-dataTabs');
            $dataTabs.find('.row.is-active').removeClass('is-active');
            $dataTabs.find('.checkbox.checkbox-checked').removeClass('checkbox-checked');
            e.stopPropagation();
        });

    };
    //日期插件初始化
    let jeDateInit = function () {
        let date = {
            format: 'YYYY-MM-DD',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59',
        };
        let dateTime = {
            format: 'YYYY-MM-DD hh:mm:ss',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59',
        }
        $('.js-date-time').jeDate(dateTime);
        $('.js-date').jeDate(date);
    };

    let render = function () {
        form.render();
        //之所以这么写是为了在flightData页面插入index页之后再获取弹框元素
        getElem();

        //首次加载获取->次日计划-离港航班数据
        pub.tableConDataInit({
            reqData: {
                aord: 'D',
                planType: 0
            },
            tmplId: 'flight-plan-tmpl',
            elemId: 'flight-plan-d'
        });
        
        //选项卡
        util.tabChange('.js-dataTabs', 'is-active');
        util.tabChange('.js-dataTable');

        //绑定事件和插件初始化
        
        flightDataTableEvent();
        modalEvent();
        menuEvent();
        dataTabsEvent();
        jeDateInit();
        //给弹框绑定layui监听事件
        pub.sendBaseDataFormVal();
        pub.sendFlightDataFormVal();
    };
    return {
        render: render
    }
})(jQuery, util, layer, layui, doT);