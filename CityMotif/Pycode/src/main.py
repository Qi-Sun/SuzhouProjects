# 所有操作的基本由本文件开始
import json
import math
import time

import pymysql

import const
import euler
import mobility
import tools

db = pymysql.connect(**const.db_conf)
cursor = db.cursor()


def city_motif_fun():
    print('正在处理城市间网络')
    # 1.0 生成城市间的网络
    # mobility.get_network_from_db_to_file(const.city_travel_route_sql, cursor, '../city_network.txt')

    # 1.1 使用 netmode 发现网络之后，从结果文件中读取 motif 矩阵
    motif_folder = '../all/'

    city_motifs_2 = ['0110']
    city_motifs_3 = tools.read_motifs_from_file(motif_folder + '3.txt', 3, 0.4, 0.1)
    city_motifs_4 = tools.read_motifs_from_file(motif_folder + '4.txt', 4, 0.4, 0.1)
    city_motifs_5 = tools.read_motifs_from_file(motif_folder + '5.txt', 5, 0.4, 0.1)
    city_motifs_6 = ['010000001000000100000010000001100000', '010000001000000100000011000100100000',
                  '010000101000000100000010000001010000']
    city_motifs_7 = ['0100000001000000010000000100000001000000011000000']
    print('读取motif完毕\n')
    # 根据 Score 和 pvalue 筛选出的所有 motif
    all_motifs = tools.combine_motifs_list(city_motifs_2, city_motifs_3, city_motifs_4, city_motifs_5, city_motifs_6,
                                           city_motifs_7)
    print('最初选定 motif 数目：', len(all_motifs))

    # 所有符合条件的 moitf 未经过排序：具有欧拉图、起点与终点一致，记录重排序之后的motif对应的原始motif
    all_fit_motifs = []
    all_fit_order_node_motif = []
    order_motif_owner_motif_dict = {}
    for x in all_motifs:
        # 对于出入度都是 1 的顶点来说，如果有欧拉图存在，则路径必定以起点为终点, 返回的欧拉图已经经过重排序
        tmp_motifs = euler.get_all_euler_ring_by_matrix_str_by_node_degree(x, 1, 1)
        if len(tmp_motifs) > 0 and x not in all_fit_motifs:
            all_fit_motifs.append(x)

        for y in tmp_motifs:
            if y not in all_fit_order_node_motif:
                all_fit_order_node_motif.append(y)
                order_motif_owner_motif_dict[y] = x     # 记录排序的 motif 的归属原始 motif
    # 上面获得了所有符合条件的未经排序的motif 以及由这些 motif 节点从排序之后的邻接矩阵字符串


    # 对符合条件的 motif 按照节点进行排序
    all_fit_motifs.sort(key=len)
    all_fit_order_node_motif.sort(key=len)
    print('符合欧拉图+首尾点一致的 motif 数目：', len(all_fit_motifs))
    print('符合条件的且经过重排序的 motif 数目：', len(all_fit_order_node_motif))
    # owner_order = {"0110": 1, "001100010": 2, "0001001010000100": 3, "001001110": 4, "0000100010010001000000100": 5,
    #                "0001000101001010": 6, "010000001000000100000010000001100000": 7, "0000100001000100100010100": 8,
    #                "0100000001000000010000000100000001000000011000000": 9, "0000100001010001000000110": 10,
    #                "0001001001011010": 11, "0001000100011110": 12, "0000100010010000010110010": 13,
    #                "0000100001000010010011010": 14, "010000101000000100000010000001010000": 15,
    #                "0000100001000100110010010": 16, "010000001000000100000011000100100000": 17}
    #
    # more_300_rst = {'0110': 865296, '010001100': 125466, '0100001000011000': 31670, '010000001000000100000010000001100000': 4456, '0100000100000100000110000': 10964, '010101010': 17784, '0100000100000101000100010': 857, '0100001010010010': 3240, '0100101000010100': 3335, '010001110': 711, '0100000100000110010010000': 924, '0100000001000000010000000100000001000000011000000': 2118, '0100000110010000000110000': 684, '0100001101001000': 2723, '0100000101000100100010000': 445, '0100010100000100000101000': 1125, '010000001000000100000011000100100000': 303, '0100101101000100': 532, '0100101001010010': 918, '010000101000000100000010000001010000': 401, '0100000100100100000100100': 665}
    # print(len(more_300_rst))
    # tmp_rst = {}
    # for x in more_300_rst:
    #     print(x, more_300_rst[x])
    #     tools.draw_image_from_matrix_str(x, True)



    # print('打印符合条件的原始 motif：')
    # for motif in all_fit_motifs:
    #     print(motif)
    # print('打印完毕\n')
    #
    # print('打印符合条件的重排序 motif：')
    # for motif in all_fit_order_node_motif:
    #     print(motif)
    # print('打印完毕\n')


    # 反查数据，根据未经重排序的 motif 获取 gID，到对应文件中进行查原值
    # {节点数目：选中的 gID 集合}
    # origin_num = {}
    # for x in all_fit_motifs:
    #     n = int(math.sqrt(len(x)))
    #     num = int(x, 2)     # gID
    #     if n not in origin_num:
    #         origin_num[n] = [num]
    #     else:
    #         origin_num[n].append(num)
    # origin_motif_data = {}
    # # 只有 3 到 5 的数据文件
    # for x in range(3, 6):
    #     rst = tools.read_motif_data_from_file(motif_folder + '%d.txt' % x, origin_num[x])
    #     for y in rst:
    #         if x not in origin_motif_data:
    #             origin_motif_data[x] = [y]
    #         else:
    #             origin_motif_data[x].append(y)
    # print(origin_num)
    # print(origin_motif_data)

    # 未经过排序和重排序之后的 motif 和旅游行为数据进行匹配，计数，统计不被记录在内的motif
    city_simple_route = mobility.get_simple_routes_from_db(const.city_travel_route_sql, cursor)
    motifs_count_rst = {}
    motifs_count_rst_order_node = {}
    fail = {}
    all_matrix = {}
    for x in city_simple_route:
        matrix_str = mobility.route_to_matrix_str(x)
        if matrix_str not in all_matrix:
            all_matrix[matrix_str] = 1
        else:
            all_matrix[matrix_str] += 1
        if matrix_str in all_fit_order_node_motif:      # 如果路线的图在选中的重排序的motif中，统计归属与重排序motif
            owner = order_motif_owner_motif_dict[matrix_str]
            if owner not in motifs_count_rst:
                motifs_count_rst[owner] = 1
            else:
                motifs_count_rst[owner] += 1

            if matrix_str not in motifs_count_rst_order_node:
                motifs_count_rst_order_node[matrix_str] = 1
            else:
                motifs_count_rst_order_node[matrix_str] += 1
        else:                                           # 如果不在其中，统计到失败选择中，作为对比筛选
            if matrix_str not in fail:
                fail[matrix_str] = 1
            else:
                fail[matrix_str] += 1
    print('路线与原始motif对应数目：', len(motifs_count_rst))
    print('路线与重排序motif对应数目：', len(motifs_count_rst_order_node))
    print('路线对应motif失败数目：', len(fail))


    # 筛选字典中值超过给定值的的 K-V，返回新字典
    def select_more_than(value_dict, min_value):
        selected = {}
        for x in value_dict:
            if value_dict[x] >= min_value:
                selected[x] = value_dict[x]
        return selected

    all_matrix= select_more_than(motifs_count_rst, 300)
    print("无差别直接统计", len(all_matrix), all_matrix)
    order_s = select_more_than(motifs_count_rst_order_node, 300)
    print("排序：", len(order_s))
    for x in order_s:
        print(x, order_s[x], order_motif_owner_motif_dict[x])
        tools.draw_image_from_matrix_str(x, True)
    print("绘画无差别统计结果")
    for x in all_matrix:
        print(x, all_matrix[x], order_motif_owner_motif_dict[x])
        tools.draw_image_from_matrix_str(x, True)

    # print('打印原始motif对应数目')
    # for motif in motifs_count_rst:
    #     print(motif, motifs_count_rst[motif])
    # print('打印结束\n')
    #
    # print('打印重排序motif对应数目')
    # for motif in motifs_count_rst_order_node:
    #     print(motif, motifs_count_rst_order_node[motif])
    # print('打印结束\n')
    #
    # print('打印匹配失败的motif对应数目')
    # for motif in fail:
    #     print(motif, fail[motif])
    # print('打印结束\n')
    # # def write_road_matrix_to_db():
    # #     cursor.execute('SELECT userid, travel_id, t_route FROM suzhou.travel_users_city_routes_3;')
    # #     data = cursor.fetchall()
    # #     print('路径数目：', len(data))
    # #     route_list = []
    # #     for x in data:
    # #         route_str = x[2]
    # #         simple_route = mobility.raw_to_simple_route(route_str)
    # #         matrix_str = mobility.route_to_matrix_str(simple_route)
    # #         n = int(math.sqrt(len(matrix_str)))
    # #         if n <= 7 and n > 0:
    # #             tmp = (x[0], x[1], matrix_str)
    # #             route_list.append(tmp)
    # #     print('计算数目：', len(route_list))
    # #     for x in route_list:
    # #         cursor.execute(
    # #             'update suzhou.travel_users_city_routes_3 set adjacent_matrix = "%s" where userid = %d and travel_id = %d' % (
    # #             x[2], x[0], x[1]))
    # #     print('写入数据库完成')
    # #
    # # write_road_matrix_to_db()

    # def write_travel_type_to_db():
    #     rst = ['0110', '001100010', '0001001010000100', '001001110', '0000100010010001000000100', '0001000101001010',
    #            '010000001000000100000010000001100000', '0000100001000100100010100',
    #            '0100000001000000010000000100000001000000011000000', '0000100001010001000000110', '0001001001011010',
    #            '0001000100011110', '0000100010010000010110010', '0000100001000010010011010',
    #            '010000101000000100000010000001010000', '0000100001000100110010010',
    #            '010000001000000100000011000100100000']
    #     tmp_motifs = {}
    #     for x in rst:
    #         tmp_order = euler.get_all_euler_path_by_matrix_str_by_node_degree(x)
    #         for y in tmp_order:
    #             tmp_motifs[y] = x
    #     type_dict = {'0110': 1, '001100010': 5, '0001001010000100': 5, '001001110': 2, '0000100010010001000000100': 5,
    #                  '0001000101001010': 4, '010000001000000100000010000001100000': 5, '0000100001000100100010100': 4,
    #                  '0100000001000000010000000100000001000000011000000': 5, '0001001001011010': 2,
    #                  '0001000100011110': 3, '0000100010010000010110010': 4, '0000100001000010010011010': 3,
    #                  '010000101000000100000010000001010000': 4}
    #
    #     cursor.execute('SELECT userid, travel_id, t_route FROM suzhou.travel_users_city_routes_3')
    #     data = cursor.fetchall()
    #     print('路径数目：', len(data))
    #     route_list = []
    #     for x in data:
    #         route_str = x[2]
    #         simple_route = mobility.raw_to_simple_route(route_str)
    #         matrix_str = mobility.route_to_matrix_str(simple_route)
    #         n = int(math.sqrt(len(matrix_str)))
    #         if n <= 7 and n > 0:
    #             tmp = (x[0], x[1], matrix_str)
    #             route_list.append(tmp)
    #     print('计算数目：', len(route_list))
    #
    #     for x in route_list:
    #         if x[2] in tmp_motifs and tmp_motifs[x[2]] in type_dict:
    #             cursor.execute(
    #                 'update suzhou.travel_users_city_routes_3 set travel_type = %d where userid = %d and travel_id = %d' % (
    #                 type_dict[tmp_motifs[x[2]]], x[0], x[1]))
    #     print('写入数据库完成')
    #
    # write_travel_type_to_db()

    # todo 1: 判断城市的目的地类型， 待处理
    # city_type_dict = {}
    # for x in city_simple_route:
    #     city_type_rst = tools.check_city_func(x)
    #     if not city_type_rst:
    #         continue
    #     for city in city_type_rst:
    #         if city not in city_type_dict:
    #             tmp = []
    #             for i in range(6):
    #                 tmp.append((i, 0))
    #             city_type_dict[city] = tmp
    #         city_type_dict[city][city_type_rst[city]] = (city_type_rst[city], city_type_dict[city][city_type_rst[city]][1] + 1)
    #
    # city_type = {0: '客源', 1: '单目的地', 2: '门户', 3: '出口', 4: '枢纽', 5: '途经'}
    #
    # for city in city_type_dict:
    #     city_type_num = city_type_dict[city][1:]
    #     city_type_num = sorted(city_type_num, key= lambda x: (x[1]), reverse=True)
    #     x = city
    #     print(x, city_type_dict[x][0][1],  city_type_dict[x][1][1],  city_type_dict[x][2][1],  city_type_dict[x][3][1],  city_type_dict[x][4][1],  city_type_dict[x][5][1], city_type[city_type_num[0][0]], city_type[city_type_num[1][0]], city_type[city_type_num[2][0]], city_type[city_type_num[3][0]], city_type[city_type_num[4][0]])

    # 附带的一些操作
    # count = 1
    # for x in const.selected_18_motif:
    #     print(count, x)
    #     tools.draw_image_from_matrix_str(x, True)
    #     count += 1

    # 把距离写入数据库
    # mobility.write_route_distance_to_db(cursor)

    # distance = mobility.load_city_distance_dict_from_db(const.city_distance_sql, cursor)
    # print(distance)


def in_city_motif_fun():
    # print('正在处理城市内部景点间网络')
    # # 生成城市内景点间的网络
    # # mobility.get_network_from_db_to_file(const.in_city_travel_route_sql, cursor, '../in_city_network.txt')
    # # 使用 netmode 发现网络之后，从结果文件中读取 motif 矩阵
    # motif_folder = '../incityall/'
    # city_motifs_2 = ['0110', '0100']
    # city_motifs_3 = tools.read_motifs_from_file(motif_folder + '3.txt', 3, 0.6, 0.1)
    # city_motifs_4 = tools.read_motifs_from_file(motif_folder + '4.txt', 4, 0.7, 0.1)
    # city_motifs_5 = tools.read_motifs_from_file(motif_folder + '5.txt', 5, 1, 0.1)
    # city_motifs_6 = tools.read_motifs_from_file(motif_folder + '6.txt', 5, 1, 0.1)
    # city_motifs_7 = ['0100000001000000010000000100000001000000010000000']
    # print('读取motif完毕\n')
    # # 根据 Score 和 pvalue 筛选出的所有 motif
    # all_motifs = tools.combine_motifs_list(city_motifs_2, city_motifs_3, city_motifs_4, city_motifs_5, city_motifs_6,
    #                                        city_motifs_7)
    # print('最初选定 motif 数目：', len(all_motifs))
    #
    # # 所有符合条件的 moitf 未经过排序：具有欧拉路径，记录重排序之后的motif对应的原始motif
    # all_fit_motifs = []
    # all_fit_order_node_motif = []
    # order_motif_owner_motif_dict = {}
    # motif_all_euler_path_dict = {}
    # done = 0
    # startTime = time.clock()
    # # 读取出motif 的所有欧拉路径
    # motifs_euler_path_json = {}
    # with open('../motif_euler_path_json.txt', 'r') as f:
    #     all_data = f.read()
    #     motifs_euler_path_json = json.loads(all_data)
    #
    #
    # for i, x in enumerate(all_motifs):
    #     # 对于出入度都是 1 的顶点来说，如果有欧拉图存在，则路径必定以起点为终点, 返回的欧拉图已经经过重排序
    #     # 找出图的所有欧拉路径，起点的出入度相同或者出度比入度多1
    #     if x in motifs_euler_path_json:
    #         tmp_motifs = motifs_euler_path_json[x]
    #     else:
    #         print('%s not in json. do the function : euler.get_all_euler_path_by_matrix_str_by_node_degree')
    #         tmp_motifs = euler.get_all_euler_path_by_matrix_str_by_node_degree(x)
    #     if len(tmp_motifs) > 0 and x not in all_fit_motifs:
    #         all_fit_motifs.append(x)
    #
    #     motif_all_euler_path_dict[x] = tmp_motifs
    #
    #     for y in tmp_motifs:
    #         if y not in all_fit_order_node_motif:
    #             all_fit_order_node_motif.append(y)
    #             order_motif_owner_motif_dict[y] = x  # 记录排序的 motif 的归属原始 motif
    #     if i - done == int(0.05 * len(all_motifs)) or i == len(all_motifs) - 1:
    #         print('查找完成度： %f%%' % (i * 100 / len(all_motifs)), time.clock() - startTime)
    #         done = i
    # # 上面获得了所有符合条件的未经排序的motif 以及由这些 motif 节点从排序之后的邻接矩阵字符串
    # print('use time : ', time.clock() - startTime)
    # # print(motif_all_euler_path_dict)
    # with open('motif_json.txt', 'w') as f:
    #     f.write(json.dumps(motif_all_euler_path_dict))
    #
    # # 对符合条件的 motif 按照节点进行排序
    # all_fit_motifs.sort(key=len)
    # all_fit_order_node_motif.sort(key=len)
    # print('符合欧拉图+首尾点一致的 motif 数目：', len(all_fit_motifs))
    # print('符合条件的且经过重排序的 motif 数目：', len(all_fit_order_node_motif))
    #
    # # print('打印符合条件的原始 motif：')
    # # for motif in all_fit_motifs:
    # #     print(motif)
    # # print('打印完毕\n')
    # #
    # # print('打印符合条件的重排序 motif：')
    # # for motif in all_fit_order_node_motif:
    # #     print(motif)
    # # print('打印完毕\n')
    #
    # # 反查数据，根据未经重排序的 motif 获取 gID，到对应文件中进行查原值
    # # {节点数目：选中的 gID 集合}
    # # origin_num = {}
    # # for x in all_fit_motifs:
    # #     n = int(math.sqrt(len(x)))
    # #     num = int(x, 2)     # gID
    # #     if n not in origin_num:
    # #         origin_num[n] = [num]
    # #     else:
    # #         origin_num[n].append(num)
    # # origin_motif_data = {}
    # # # 只有 3 到 5 的数据文件
    # # for x in range(3, 6):
    # #     rst = tools.read_motif_data_from_file(motif_folder + '%d.txt' % x, origin_num[x])
    # #     for y in rst:
    # #         if x not in origin_motif_data:
    # #             origin_motif_data[x] = [y]
    # #         else:
    # #             origin_motif_data[x].append(y)
    # # print(origin_num)
    # #
    # # size = 5
    # #
    # # print(origin_motif_data[size])
    # #
    # # # 筛选字典中值超过给定值的的 K-V，返回新字典
    # def select_more_than(value_dict, min_value):
    #     selected = {}
    #     for x in value_dict:
    #         if value_dict[x] >= min_value:
    #             selected[x] = value_dict[x]
    #     return selected
    #
    # count_rst = const.incity_count_dict
    # min_value = 300
    # more_min_value = select_more_than(count_rst, min_value)
    # print('超过 %d 的motif有' % min_value, len(more_min_value), more_min_value)
    # euler_motif = {}
    # for x in more_min_value:
    #     if euler.have_euler_path(x):
    #         euler_motif[x] = more_min_value[x]
    #         # tools.draw_image_from_matrix_str(x, True)
    #
    # print('拥有欧拉路径的有 %d' % len(euler_motif))
    #
    # # print(origin_motif_data[size])
    # #
    # # for x in origin_motif_data[size]:
    # #     matrix_str = tools.num_to_motif_matrix(x['gID'], size)
    # #     tmp_order_motif = euler.get_all_euler_path_by_matrix_str_by_node_degree(matrix_str)
    # #     for y in tmp_order_motif:
    # #         if y in count_rst and count_rst[y] >= 300:
    # #             print(matrix_str, y, x['gID'], count_rst[y])
    #
    # # 未经过排序和重排序之后的 motif 和旅游行为数据进行匹配，计数，统计不被记录在内的motif
    # city_simple_route = mobility.get_simple_routes_from_db(const.in_city_travel_route_sql, cursor)
    # print('旅游行为数目：', len(city_simple_route))
    # motifs_count_rst = {}
    # motifs_count_rst_order_node = {}
    # # fail = {}
    # # tmp_selected_motif = {}
    # # done = 0
    # # startTime = time.clock()
    # # delta = int(0.05 * len(city_simple_route))
    # # for i, x in enumerate(city_simple_route):
    # #     matrix_str = mobility.route_to_matrix_str(x)
    # #
    # #     if matrix_str not in tmp_selected_motif:
    # #         tmp_selected_motif[matrix_str] = 1
    # #     else:
    # #         tmp_selected_motif[matrix_str] += 1
    # #
    # #     if matrix_str in all_fit_order_node_motif:  # 如果路线的图在选中的重排序的motif中，统计归属与重排序motif
    # #         owner = order_motif_owner_motif_dict[matrix_str]
    # #         if owner not in motifs_count_rst:
    # #             motifs_count_rst[owner] = 1
    # #         else:
    # #             motifs_count_rst[owner] += 1
    # #
    # #         if matrix_str not in motifs_count_rst_order_node:
    # #             motifs_count_rst_order_node[matrix_str] = 1
    # #         else:
    # #             motifs_count_rst_order_node[matrix_str] += 1
    # #     else:  # 如果不在其中，统计到失败选择中，作为对比筛选
    # #         if matrix_str not in fail:
    # #             fail[matrix_str] = 1
    # #         else:
    # #             fail[matrix_str] += 1
    # #     if i - done == delta or i == len(city_simple_route) - 1:
    # #         print('匹配路径完成度： %2.2f%%' % (i * 100 / len(city_simple_route)), time.clock() - startTime)
    # #         done = i
    # # target_motif = ['0100', '010001000', '0100001000010000', '0100000100000100000100000']
    # # target_motif = ['010001000', '0100001000010000', '0100000100000100000100000']
    # # target_motif = ['0110', '010001100', '010001010']
    # target_motif = ['0100001000010000']
    # target_routes = []
    # for route in city_simple_route:
    #     matrix_str = mobility.route_to_matrix_str(route)
    #     if matrix_str in target_motif:# and route not in target_routes:
    #         target_routes.append(route)
    # print('目标路线有 %d' % len(target_routes))
    # k = 4
    # # mobility.new_k_mean(target_routes, k, {})
    # k_rst = []
    # for i in range(200):
    #     try:
    #         print("进行第%d次kmean聚类" % i)
    #         tmp_r = mobility.vector_k_mean(target_routes, k)
    #         k_rst.append(tmp_r)
    #     except Exception as e:
    #         print(e)
    #         continue
    # k_rst.sort(key=lambda x: x[0])
    # print(k_rst[0])
    # for i in k_rst:
    #     print(i[0], i[1])
    #
    # # best
    # # [['平江路', '观前街', '金鸡湖', '山塘'], ['山塘', '寒山寺', '虎丘山', '拙政园'], ['平江路', '观前街', '拙政园', '苏州博物馆'], ['拙政园', '狮子林', '观前街', '寒山寺']]
    #
    # # print('路线与原始motif对应数目：', len(motifs_count_rst))
    # # print('路线与重排序motif对应数目：', len(motifs_count_rst_order_node))
    # # print('路线对应motif失败数目：', len(fail))
    # #
    # # print('临时统计结果：', len(tmp_selected_motif))
    # # for motif in tmp_selected_motif:
    # #     print(motif, tmp_selected_motif[motif])
    # # tmp_rst = select_more_than(tmp_selected_motif, 300)
    # # print('筛选 300 的moitf数目：', len(tmp_rst))
    # # for motif in tmp_rst:
    # #     print(motif, tmp_rst[motif])
    # #     tools.draw_image_from_matrix_str(motif, True)
    #
    # print('打印原始motif对应数目')
    # for motif in motifs_count_rst:
    #     print(motif, motifs_count_rst[motif])
    # print('打印结束\n')
    #
    # print('打印重排序motif对应数目')
    # for motif in motifs_count_rst_order_node:
    #     print(motif, motifs_count_rst_order_node[motif])
    # print('打印结束\n')
    # #
    # # print('打印匹配失败的motif对应数目')
    # # for motif in fail:
    # #     print(motif, fail[motif])
    # # print('打印结束\n')
    #
    # # # 重新计算以下每条路径中节点的数目
    # # def check_place_num_to_db():
    # #     cursor.execute('SELECT userid, action_id, t_route FROM suzhou.suzhou_action_sq_new where t_route is not null')
    # #     data = cursor.fetchall()
    # #     print('路径数目：', len(data))
    # #     route_list = []
    # #     for x in data:
    # #         route_str = x[2]
    # #         simple_route = mobility.raw_to_simple_route(route_str)
    # #         matrix_str = mobility.route_to_matrix_str(simple_route)
    # #         n = int(math.sqrt(len(matrix_str)))
    # #         tmp = (x[0], x[1], n)
    # #         route_list.append(tmp)
    # #     print('计算数目：', len(route_list))
    # #     for x in route_list:
    # #         cursor.execute('update suzhou.suzhou_action_sq_new set place_num_new_tqh = %d where userid = %d and action_id = %d' % (x[2], x[0], x[1]))
    # #     print('写入数据库完成')


    def write_road_matrix_to_db():
        matrix_travel_type = {'0': 0, '0110': 1, '0100': 1, '011100000': 3, '010001010': 2, '010001100': 2, '010001000 ': 1, '0100001000010000': 1, '0100000100000100000100000': 1}
        cursor.execute('SELECT userid, action_id, t_route FROM suzhou.suzhou_action_sq_new where t_route is not null')
        data = cursor.fetchall()
        print('路径数目：', len(data))
        route_list = []
        for x in data:
            route_str = x[2]
            simple_route = mobility.raw_to_simple_route(route_str)
            matrix_str = mobility.route_to_matrix_str(simple_route)
            n = int(math.sqrt(len(matrix_str)))
            if n <= 7 and n > 0:
                tmp = (x[0], x[1], matrix_str)
                route_list.append(tmp)
        print('计算数目：', len(route_list))
        for x in route_list:
            if x[2] not in matrix_travel_type:
                cursor.execute('update suzhou.suzhou_action_sq_new set adjacent_matrix = %s where userid = %d and action_id = %d' % (x[2], x[0], x[1]))
            else:
                cursor.execute(
                    'update suzhou.suzhou_action_sq_new set adjacent_matrix = %s, travel_type = %d where userid = %d and action_id = %d' % (
                    x[2], matrix_travel_type[x[2]], x[0], x[1]))
        print('写入数据库完成')

    # write_road_matrix_to_db()

    def write_simple_road_matrix_to_db():
        cursor.execute('SELECT userid, action_id, t_route FROM suzhou.suzhou_action_sq_new where t_route is not null')
        data = cursor.fetchall()
        print('路径数目：', len(data))
        route_list = []
        for x in data:
            route_str = x[2]
            simple_route = mobility.raw_to_simple_route(route_str)
            if len(simple_route) > 0:
                s_route = ';'.join(simple_route)
                tmp = (x[0], x[1], s_route)
                route_list.append(tmp)
        print('计算数目：', len(route_list))
        for x in route_list:
            cursor.execute(
                'update suzhou.suzhou_action_sq_new set s_t_route = "%s" where userid = %d and action_id = %d' % (
                x[2], x[0], x[1]))
        print('写入数据库完成')

    write_simple_road_matrix_to_db()
    # check_place_num_to_db()


city_motif_fun()