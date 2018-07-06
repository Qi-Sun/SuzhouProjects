import datetime
import math
import operator
import os
import random
import time

import pymysql

import const
import tools

host = 'localhost'
port = 3306
user = 'root'
password = '19950310'
db_name = 'suzhou'
charset = 'utf8'
db = pymysql.connect(host=host, port=port, user=user, password=password, db=db_name, charset=charset)
cursor = db.cursor()

tourism_action = 'SELECT t_route FROM suzhou.suzhou_action_sq_new where t_route is not null and place_num_new > 1;'
poi_type = 'SELECT distinct super_a, type_new FROM suzhou.poi_tourism_place;'
userid_actionid_t_route = 'SELECT userid, action_id, t_route FROM suzhou.suzhou_action_sq_new where t_route is not ' \
                          'null and place_num_new > 0;'
sql_list = [
    'SELECT t_route FROM suzhou.suzhou_action_sq_new where t_route is not null and place_num_new > 1 and userid '
    'in (select uid from tourism_id);',
    'SELECT t_route FROM suzhou.suzhou_action_sq_new where t_route is not null and place_num_new > 1 and userid '
    'in (select uid from tourism_id where user_type = 0);',
    'SELECT t_route FROM suzhou.suzhou_action_sq_new where t_route is not null and place_num_new > 1 and userid '
    'in (select uid from tourism_id where user_type = 1);',
    'SELECT t_route FROM suzhou.suzhou_action_sq_new where t_route is not null and place_num_new > 1 and userid '
    'in (select uid from tourism_id where user_type = 2);']
motif_id_tuple_list = [('0', 0), ('0100', 1), ('0110', 2), ('010001000', 3), ('010001100', 4), ('011100000', 5),
                       ('010001010', 6), ('011100100', 7), ('010101010', 8), ('0100001000010000', 9),
                       ('0100001000011000', 10), ('0110100000010000', 11), ('0101001010000000', 12),
                       ('0100001101000000', 13), ('0100001000010010', 14), ('0100001000010100', 15),
                       ('0100000100000100000100000', 16), ('0100000100000100000110000', 17),
                       ('0110010000000100000100000', 18), ('0101000100100000000100000', 19),
                       ('0100100100000101000000000', 20), ('0100000110010000000100000', 21),
                       ('0100000100000110010000000', 22), ('0100000100000100000100010', 23),
                       ('0100000100000100000100100', 24), ('0100000100000100000101000', 25),
                       ('010000001000000100000010000001000000', 26),
                       ('0100000001000000010000000100000001000000010000000', 27),
                       ('0100000000100000000100000000100000000100000000100000000100000000', 28)]
motif_id_dict = dict(motif_id_tuple_list)
motif_type = [('0', 0), ('0100', 1), ('0110', 2), ('010001000', 1), ('010001100', 2), ('011100000', 3),
              ('010001010', 5), ('011100100', 6), ('010101010', 6), ('0100001000010000', 1), ('0100001000011000', 2),
              ('0110100000010000', 3), ('0101001010000000', 3), ('0100001101000000', 4), ('0100001000010010', 5),
              ('0100001000010100', 5), ('0100000100000100000100000', 1), ('0100000100000100000110000', 2),
              ('0110010000000100000100000', 3), ('0101000100100000000100000', 3), ('0100100100000101000000000', 3),
              ('0100000110010000000100000', 4), ('0100000100000110010000000', 4), ('0100000100000100000100010', 5),
              ('0100000100000100000100100', 5), ('0100000100000100000101000', 5),
              ('010000001000000100000010000001000000', 1), ('0100000001000000010000000100000001000000010000000', 1),
              ('0100000000100000000100000000100000000100000000100000000100000000', 1)]
matrix_str_motif_type_dict = dict(motif_type)
chain_matrix_str = ['0100', '010001000', '0100001000010000', '0100000100000100000100000',
                    '010000001000000100000010000001000000', '0100000001000000010000000100000001000000010000000',
                    '0100000000100000000100000000100000000100000000100000000100000000']
output_folder = 'tmp_save/'
model_dis_dict_filename = output_folder + 'model_dis_dict_rst'


def del_null(place_list):
    count = place_list.count('NULL')
    for x in range(0, count):
        place_list.remove('NULL')
    count = place_list.count('')
    for x in range(0, count):
        place_list.remove('')
    return place_list


def combine_neighbor(l):
    length = len(l)
    for x in range(length - 1, 0, -1):
        if l[x] == l[x - 1]:
            del l[x]
    return l


def raw_to_simple_route(raw):
    route = raw.split(';')
    route = del_null(route)
    return combine_neighbor(route)


# 根据 sql 查询所有符合条件的路径，生成简单路径，sql 返回结果只有路径那一列
def get_simple_routes_from_db(sql, db_cursor):
    '''
    根据 sql 查询所有符合条件的路径，生成简单路径，sql 返回结果只有路径那一列
    :param sql:查询路径的字符串
    :param db_cursor: 连接了数据库的 cursor
    :return:路径的集合
    '''
    rst = []
    assert isinstance(sql, str)
    db_cursor.execute(sql)
    route_str_data = db_cursor.fetchall()
    for route_str_tuple in route_str_data:
        route_str = route_str_tuple[0]
        simple_route = raw_to_simple_route(route_str)
        if simple_route:
            rst.append(simple_route)
    return rst


# 根据给定路线生成地点与标号的字典以及标号与地点对应关系的字典
def get_places_dicts_in_all_routes(simple_routs):
    places = {}
    index_place = {}
    num = 1
    for arr in simple_routs:
        for place in arr:
            if place not in places:
                places[place] = num
                index_place[num] = place
                num += 1
    return places


# 根据路线列表生成网络，返回边的字典，键为边元组(s, e) 和顶点数目
def network(simple_routes):
    places = get_places_dicts_in_all_routes(simple_routes)
    edges = []
    for route in simple_routes:
        length = len(route)
        for i in range(0, length - 1):
            tmp_key = (places[route[i]], places[route[i + 1]])
            if tmp_key not in edges:
                edges.append(tmp_key)
    edges = sorted(edges, key=lambda x: (x[0], x[1]))
    return edges, len(places)


# 从路径生成相邻矩阵字符串
def route_to_matrix_str(route):
    assert len(route) > 0
    places = {}
    num = 0
    for x in route:
        if x not in places:
            places[x] = num
            num += 1
    n = len(places)
    length = n * n
    mat = ['0'] * length
    if length == 1:  # 只有一个点时
        return '0'
    for i in range(0, len(route) - 1):
        row = places[route[i]]
        col = places[route[i + 1]]
        mat[row * n + col] = '1'
    return ''.join(mat)


# 给字符串添加时间后缀，用于生成文件时容易看出时间
def get_filename_time(filename):
    now = datetime.datetime.now()
    return filename + str(now.year) + str(now.month) + str(now.day) + str(now.hour) + str(now.minute) + str(now.second)


# 计算两条路线中相同地点出现次数
def same_place_num(route1, route2):
    same_num = 0
    for x in route1:
        if x in route2:
            same_num += 1
    return 1 - 2 * same_num / (len(route1) + len(route2))


# 计算两个路径之间的最长公共子序列长度
def longest_common_sub_sequence(route1, route2):
    m = len(route1)
    n = len(route2)
    rst = []
    for i in range(0, m + 1):
        tmp = [0] * (n + 1)
        rst.append(tmp)
    for i in range(1, m + 1):
        for j in range(1, n + 1):
            if route1[i - 1] == route2[j - 1]:
                rst[i][j] = rst[i - 1][j - 1] + 1
            elif rst[i - 1][j] >= rst[i][j - 1]:
                rst[i][j] = rst[i - 1][j]
            else:
                rst[i][j] = rst[i][j - 1]
    return rst[m][n]


# 计算两条路径的距离
# def dx(route1, route2):
#     m = longest_common_sub_sequence(route1, route2)
#     new_route1 = route1[:]
#     new_route1.reverse()
#     n = longest_common_sub_sequence(new_route1, route2)
#     n = max(m, n)
#     delx = 1 - 2 * n / (len(route1) + len(route2))
#     return delx * delx


def dx(route1, route2):
    m = longest_common_sub_sequence(route1, route2)
    new_route1 = route1[:]
    new_route1.reverse()
    n = longest_common_sub_sequence(new_route1, route2)
    n = max(m, n)
    if n == 0:
        return (len(route1) + len(route2)) / 2
    else:
        return (len(route1) + len(route2)) / 2 / n - 1


# 寻找最近的中心，用于聚类
def find_nearest_center(k_center, route):
    dis = 100
    index = 0
    for i in range(0, len(k_center)):
        i_dis = dx(k_center[i], route)
        # assert i_dis <= 1
        if i_dis < dis:
            dis = i_dis
            index = i
    return index


# 计算聚类结果的新的中心
def calculate_center(cluster):
    n = len(cluster)
    dis_list = []
    count = 0
    done = 0
    for route1 in cluster:
        tmp_dis = 0
        for route2 in cluster:
            tmp_dis += dx(route1, route2)
        dis_list.append(tmp_dis)
        count += 1
        if count / n - done > 0.05:
            done = count / n
            print(done)
    min_dis = min(dis_list)
    index = dis_list.index(min_dis)
    return cluster[index]


# 给定路径的 sql 和连接数据库的 cursor, 生成网络，写入文件中
def get_network_from_db_to_file(sql, db_cursor, filename):
    simple_routes = get_simple_routes_from_db(sql, db_cursor)
    edges, place_num = network(simple_routes)  # 生成网络边
    with open(filename, 'w') as f:  # 将有向边写入文件中，然后使用 FanMod/netmode 进行 motif 发现，注意文件格式
        f.write(str(place_num) + '\n')
        for edge in edges:
            edge_str = str(edge[0]) + ' ' + str(edge[1])
            f.write(edge_str + "\n")


# -------------------------------------------------------------------------------------------------------------------- #


# 从数据库原始的路线数据生成线路矩阵，即 motif 的对应矩阵字符串表示，返回矩阵字符串以及简化路线
def raw_to_matrix_str_and_simple_route(raw):
    tmp = raw[0][:-1]
    route = raw_to_simple_route(tmp)
    matrix_str = route_to_matrix_str(route)
    return matrix_str, route


# 通过 sql 语句选择出行为，再根据给出的矩阵选择对应 motifs 类型的行为，返回简化路径列表
def get_db_simple_route_by_matrix_str_list(user_sql, matrix_strs):
    cursor.execute(user_sql)
    data = cursor.fetchall()
    rst = []
    for raw in data:
        matrix_str, simple_route = raw_to_matrix_str_and_simple_route(raw)
        if matrix_str in matrix_strs:
            rst.append(simple_route)
    return rst


# 根据模型距离字典，快速查找聚类的新中心
def calculate_center_by_model_dict(cluster, model_dict):
    n = len(cluster)
    dis_list = []
    count = 0
    done = 0
    for route in cluster:
        tmp_dis = 0
        for another_route in cluster:
            model_str = routes_str_sequential_relations(route, another_route)
            if model_str in model_dict:
                tmp_dis += model_dict[model_str]
            else:
                dis = dx(route, another_route)
                # print(model_dict)
                # print(model_str)
                model_dict[model_str] = dis
                tmp_dis += dis
        dis_list.append(tmp_dis)
        count += 1
        if count / n - done > 0.05 or count == n:
            done = count / n
            print('计算簇中心：%.2f%%' % (done * 100))
    min_dis = min(dis_list)
    index = dis_list.index(min_dis)
    return cluster[index], model_dict


def load_model_dis_dict_from_file(filename):
    if not os.path.exists(filename):
        return {}
    with open(filename, 'r') as f:
        dicts = {}
        while True:
            tmp = f.readline()
            if tmp == '':
                break
            dicts[tmp.split(' ')[0]] = float(tmp.split(' ')[1])
        print('load dict from file finish.')
        return dicts


def write_model_dis_dict_to_file(dis_dict, filename):
    with open(filename, 'w') as f:
        for x in dis_dict:
            f.write(str(x) + ' ' + str(dis_dict[x]) + '\n')
    print('write dict to file finish.')


# -------------------------------------
# 生成旅游路线对应 motif 的数目，即求路线中某个 motif 的数目，从两个点的 motif 开始
def main_generate_network():
    print('working...')
    cursor.execute(tourism_action)
    data = cursor.fetchall()
    count = len(data)
    simple_routes = []
    for x in range(0, count):
        tmp = data[x][0][:-1]
        simple_route = raw_to_simple_route(tmp)
        if len(simple_route) > 1:
            simple_routes.append(simple_route)  # 从原始路线数据转成精简路径，合并相邻点，去掉合并后只有一个点的行为，所以路线中可能有3个地点却会保留5个边

    edges, place_num = network(simple_routes)  # 生成网络边
    with open("network.txt", 'w') as f:  # 将有向边写入文件 network.txt 中，然后使用 FanMod 进行 motif 发现
        f.write(str(place_num) + '\n')
        for edge in edges:
            edge_str = str(edge[0]) + ' ' + str(edge[1])
            f.write(edge_str + "\n")
    return simple_routes


# 统计 motif 对应路径数目
def main_count_motifs(simple_routes):
    # motif_tuple 是发现 motif 之后生成的邻接矩阵
    motif_tuple = [('0100', 1), ('0110', 2), ('010001000', 3), ('010001100', 4), ('011100000', 5), ('010001010', 6),
                   ('011100100', 7), ('010101010', 8), ('0100001000010000', 9), ('0100001000011000', 10),
                   ('0110100000010000', 11), ('0101001010000000', 12), ('0100001101000000', 13),
                   ('0100001000010010', 14), ('0100001000010100', 15), ('0100000100000100000100000', 16),
                   ('0100000100000100000110000', 17), ('0110010000000100000100000', 18),
                   ('0101000100100000000100000', 19), ('0100100100000101000000000', 20),
                   ('0100000110010000000100000', 21), ('0100000100000110010000000', 22),
                   ('0100000100000100000100010', 23), ('0100000100000100000100100', 24),
                   ('0100000100000100000101000', 25), ('010000001000000100000010000001000000', 26),
                   ('0100000001000000010000000100000001000000010000000', 27),
                   ('0100000000100000000100000000100000000100000000100000000100000000', 28)]
    motif_dict = dict(motif_tuple)
    for motif in motif_dict:
        motif_dict[motif] = 0

    bad_route = []  # 生成‘坏’的路线，即没有被归类的路线
    for route in simple_routes:
        motif_matrix_str = route_to_matrix_str(route)
        if motif_matrix_str in motif_dict:
            motif_dict[motif_matrix_str] += 1
        else:
            bad_route.append(route)
    with open('motif_result.txt', 'w') as f:
        for motif in motif_dict:
            f.write(motif + ':' + str(motif_dict[motif]) + '\n')

    print("motif 如下：" + str(motif_dict))
    with open('not_in_motifs_result.txt', 'w') as f:
        for route in bad_route:
            f.write(';'.join(route) + '\n')
    print('data save in motif_result.txt')


# 生成网络，并根据内置的 motifs 矩阵统计符合 motifs 的路径数目
def main_motifs_routes_count():
    simple_routes = main_generate_network()
    main_count_motifs(simple_routes)


# -------------------------------------


# 从数据库获取路线数据，并把对应的 motif 类型写入数据库
def write_motif_to_data_set():
    cursor.execute(userid_actionid_t_route)
    data = cursor.fetchall()
    dbdata = []
    for x in data:
        x_tuple = [x[0], x[1]]
        tmp = x[2][:-1]
        simple_route = raw_to_simple_route(tmp)
        x_tuple.append(simple_route)
        dbdata.append(x_tuple)
    # 路线已合并
    for route in dbdata:
        route.append(route_to_matrix_str(route[2]))
        if route[3] in motif_id_dict:
            motif_type_id = matrix_str_motif_type_dict[route[3]]
        else:
            motif_type_id = -1
        route.append(motif_type_id)
    with open('tourism_motif.txt', 'w') as f:
        for x in dbdata:
            f.write(str(x[0]) + ' ' + str(x[1]) + ' ' + str(x[4]) + '\n')
    count = 0
    total = len(dbdata)
    done = 0
    for x in dbdata:
        sql = 'update suzhou.suzhou_action_sq_new set motif_type = ' + str(x[4]) + ' where userid = ' + str(
            x[0]) + ' and action_id = ' + str(x[1]) + ';'
        cursor.execute(sql)
        count += 1
        if count / total - done > 0.05:
            done = count / total
            print(done)


# 生成旅游路线对应 motif 的数目
def route_motif_different_source_num():
    print('生成不同 motif 客源差异')
    local = []
    not_local = []
    around = []
    print('正在获取三个客源的用户 id')
    cursor.execute('SELECT uid FROM suzhou.tourism_id where user_type = 0;')
    local_user = cursor.fetchall()
    for uid in local_user:
        local.append(uid[0])
    cursor.execute('SELECT uid FROM suzhou.tourism_id where user_type = 1;')
    not_local_user = cursor.fetchall()
    for uid in not_local_user:
        not_local.append(uid[0])
    cursor.execute('SELECT uid FROM suzhou.tourism_id where user_type = 2;')
    around_user = cursor.fetchall()
    for uid in around_user:
        around.append(uid[0])
    print('获取三个客源的用户 id 完成')
    # 获取游客的轨迹
    print('开始从旅游行为到行为矩阵的转化')
    cursor.execute(
        'SELECT userid, t_route FROM suzhou.suzhou_action_sq_new where t_route is not null and place_num_new >= 1;')
    tourism_data = []
    tourism_acts = cursor.fetchall()
    for action in tourism_acts:
        tmp = [action[0]]
        # 获取路径的 motif 代码以及简化路径
        s_route = raw_to_simple_route(action[1][:-1])
        motif_str = route_to_matrix_str(s_route)
        tmp.append(motif_str)
        tourism_data.append(tmp)
    print('完成旅游行为到行为矩阵的转化')
    motif_dict = dict(motif_type)
    for x in motif_dict:
        # 把 motif 矩阵对应到三个客源上，先置为 0
        motif_dict[x] = [0, 0, 0]
    print('对路线的矩阵进行判定，并对应到三个客源上')
    count = len(tourism_data)
    for i, data in enumerate(tourism_data):
        if data[1] in motif_dict:
            if data[0] in local:
                motif_dict[data[1]][0] += 1
            elif data[0] in not_local:
                motif_dict[data[1]][1] += 1
            if data[0] in around:
                motif_dict[data[1]][2] += 1
        if i % 10000 == 0 or i == count:
            print('finish %.2f%%' % (100 * i / count))

    with open('motif_diff_source.txt', 'w') as f:
        for x in motif_dict:
            f.write(
                str(x) + ' ' + str(motif_dict[x][0]) + ' ' + str(motif_dict[x][1]) + ' ' + str(motif_dict[x][2]) + '\n')


# 对 k_means 分类结果中的所给路径集合生成矩阵，分别统计每一个聚类结果中三个客源的数目
def count_k_means_rst_to_diff_source(k_mean_cluster, save_path):
    print('生成不同 motif 客源差异')
    local = []
    not_local = []
    around = []
    print('正在获取三个客源的用户 id')
    cursor.execute('SELECT uid FROM suzhou.tourism_id where user_type = 0;')
    local_user = cursor.fetchall()
    for uid in local_user:
        local.append(uid[0])
    cursor.execute('SELECT uid FROM suzhou.tourism_id where user_type = 1;')
    not_local_user = cursor.fetchall()
    for uid in not_local_user:
        not_local.append(uid[0])
    cursor.execute('SELECT uid FROM suzhou.tourism_id where user_type = 2;')
    around_user = cursor.fetchall()
    for uid in around_user:
        around.append(uid[0])
    print('获取三个客源的用户 id 完成')
    # 获取游客的轨迹
    print('开始转化成简单行为')
    cursor.execute(
        "SELECT userid, t_route FROM suzhou.suzhou_action_sq_new where t_route is not null and place_num_new >= 1;")
    tourism_data = []
    tourism_acts = cursor.fetchall()
    for action in tourism_acts:
        tmp = [action[0]]
        # 获取路径的 motif 代码以及简化路径
        route = raw_to_simple_route(action[1][:-1])
        tmp.append(route)
        tourism_data.append(tmp)
    print('转化成简单行为结束')

    motif_dict = {}
    for i in range(0, len(k_mean_cluster)):
        motif_dict[i] = [0, 0, 0]
    print('对路线的矩阵进行判定，并对应到三个客源上')
    count = len(tourism_data)
    for i, data in enumerate(tourism_data):
        for j in range(0, len(k_mean_cluster)):
            if data[1] in k_mean_cluster[j]:
                if data[0] in local:
                    motif_dict[j][0] += 1
                elif data[0] in not_local:
                    motif_dict[j][1] += 1
                if data[0] in around:
                    motif_dict[j][2] += 1
                break
        if i % 10000 == 0 or i == count:
            print('finish %.2f%%' % (100 * i / count))

    with open(save_path, 'w') as f:
        for x in motif_dict:
            f.write(
                str(x) + ' ' + str(motif_dict[x][0]) + ' ' + str(motif_dict[x][1]) + ' ' + str(motif_dict[x][2]) + '\n')
    with open(get_filename_time(save_path), 'w') as f:
        for x in motif_dict:
            f.write(
                str(x) + ' ' + str(motif_dict[x][0]) + ' ' + str(motif_dict[x][1]) + ' ' + str(motif_dict[x][2]) + '\n')


# 提供模型距离字典，对路线进行聚类，最终返回最终中心以及聚类的结果列表，并把模型距离字典写入 model_dis_dict_filename 的值开头的文件中
def k_mean_fast_with_dis_dict(routes, k, model_dis_dict):
    old_k_center = []
    seed_center = {}
    # for route in routes:
    #     seed = ' '.join(route)
    #     if seed not in seed_center:
    #         seed_center[seed] = 1
    #     if len(seed_center) == k:
    #         break
    while True:
        tmp_int = random.randint(0, len(routes))
        seed = ' '.join(routes[tmp_int][:])
        if seed not in seed_center:
            seed_center[seed] = 1
        if len(seed_center) == k:
            break
    # 获取开始的 k 个种子点
    for seed in seed_center:
        old_k_center.append(seed.split(' '))
    count = 1
    print('正在进行第 ' + str(count) + ' 次迭代')

    match_routes = []
    for x in range(0, k):
        match_routes.append([])
    for route in routes:
        index = find_nearest_center(old_k_center, route)
        match_routes[index].append(route)
    new_seed_center = []
    print('匹配完成，寻找新种子点')
    for i, x in enumerate(match_routes):
        new_seed, model_dis_dict = calculate_center_by_model_dict(x, model_dis_dict)
        new_seed_center.append(new_seed)
    print("新中心：" + str(new_seed_center))
    is_done = not operator.eq(new_seed_center, old_k_center)
    old_centers = []
    while is_done:
        count += 1
        print('正在进行第 ' + str(count) + ' 次迭代')
        old_centers.append(old_k_center[:])
        old_k_center = new_seed_center
        new_seed_center = []
        match_routes = []
        for x in range(0, k):
            match_routes.append([])
        for route in routes:
            index = find_nearest_center(old_k_center, route)
            match_routes[index].append(route)
        for i, x in enumerate(match_routes):
            new_seed, model_dis_dict = calculate_center_by_model_dict(x, model_dis_dict)
            new_seed_center.append(new_seed)
        print("新中心：" + str(new_seed_center))
        if operator.eq(new_seed_center, old_k_center):
            print('种子点不变化，结束程序')
            is_done = False
        else:
            for center in old_centers:
                if operator.eq(new_seed_center, center):
                    is_done = False
                    print('进入循环，退出')
                    break

    write_model_dis_dict_to_file(model_dis_dict, model_dis_dict_filename)
    tmp = get_filename_time(model_dis_dict_filename)
    write_model_dis_dict_to_file(model_dis_dict, tmp)
    print('共进行了 ' + str(count) + ' 次迭代')
    print(new_seed_center)
    return new_seed_center, match_routes


# 将两条路径关系序列化成为字符串，用于生成模式，避免重复计算距离
def routes_str_sequential_relations(route1, route2):
    if len(route1) >= len(route2):
        long_route = route1[:]
        short_route = route2[:]
    else:
        long_route = route2[:]
        short_route = route1[:]
    char_list = [chr(i) for i in range(65, 91)]
    char_list.reverse()
    places = {}
    rst = ''
    for place in long_route:
        if place not in places:
            places[place] = char_list.pop()
            rst += places[place]
        else:
            rst += places[place]
    rst += '-'
    for place in short_route:
        if place not in places:
            places[place] = char_list.pop()
            rst += places[place]
        else:
            rst += places[place]
    # print(rst)
    return rst


# 对不同的客源的路径进行 k_means 聚类
def diff_source_k_means(sql_list, k):
    # k = 3
    save_file_name = output_folder + 'diff_k_means_' + str(k)
    for sql in sql_list:
        start = time.clock()
        chain_routes = get_db_simple_route_by_matrix_str_list(sql, chain_matrix_str)
        model_dis_dict = load_model_dis_dict_from_file('modeldisdictrst.txt')
        rst_seed, rst_cluster = k_mean_fast_with_dis_dict(chain_routes[:], k, model_dis_dict)
        end = time.clock()
        with open(save_file_name, 'a') as f:
            f.write(sql + '\n')
            f.write(str(rst_seed) + '\n')
            f.write(str(rst_cluster) + '\n')
            f.write('use %d s' % (end - start) + '\n\n')
        print('save cluster seeds and result in ' + save_file_name)
        print('finish users from：' + sql)
        # 按时间生成文件，防止误写
        with open(get_filename_time(save_file_name), 'a') as f:
            f.write(sql + '\n')
            f.write(str(rst_seed) + '\n')
            f.write(str(rst_cluster) + '\n')
            f.write('use %d s' % (end - start) + '\n\n')


def get_user_time_city_by_id(user_id):
    return 'SELECT time, cityname FROM suzhou.travel_poi_users_weibodata_suzhou where userid = ' + str(
        user_id) + ' order by time;'


def generate_city_routes(interval):
    tourism_id_sql = 'SELECT uid FROM suzhou.tourism_id;'
    cursor.execute(tourism_id_sql)
    users_data = cursor.fetchall()
    users = {}
    # 获取游客 id
    for row in users_data:
        users[row[0]] = []

    print("正在获取用户的时间和城市的列表")
    # 获取用户的时间和城市的列表
    for user_id in users:
        cursor.execute(get_user_time_city_by_id(user_id))
        user_time_city = cursor.fetchall()
        for row in user_time_city:
            users[user_id].append([row[0], row[1]])
    print("获取获取用户的时间和城市的列表完毕")
    # 按时间间隔分开成不同的旅行
    # todo: 是否将长居地作为起始点，截掉最初不符合长居地的地点
    print(users)
    users_routes = {}
    for user_id in users:
        user_id_routes = []
        tmp_route = []
        for i in range(0, len(users[user_id]) - 1):
            start_t = users[user_id][i][0]
            end_t = users[user_id][i + 1][0]
            delta_t = end_t - start_t
            if delta_t <= interval:
                tmp_route.append(users[user_id][1])
            else:
                user_id_routes.append(tmp_route)
                tmp_route = []
        users_routes[user_id] = user_id_routes
        print(user_id_routes)
        print('生成路径结束')


# 1. 去头去尾
# 将第一个长居地之前部分去掉，将最后一个长居地之后部分去掉
def del_head_tail_of_city_list(city_list, residence_city):
    head = 0
    tail = len(city_list)
    for index, city in enumerate(city_list):
        if city[1] == residence_city:
            head = index
            break
    tmp_city_list = city_list[:]
    tmp_city_list.reverse()
    for index, city in enumerate(tmp_city_list):
        if city[1] == residence_city:
            tail = len(city_list) - index
            break
    return city_list[head:tail]


# 删除 时间-城市 列表中出现的错误城市名， 如 "不是苏州市"
def del_error_city(city_list, error_city_name):
    rst = city_list[:]
    for i in range(len(rst) - 1, -1, -1):
        if rst[i][1] == error_city_name:
            del rst[i]
    return rst


# 2. 去除无用重复长居地
# 将连续出现的多个长居地中间去掉，所以列表中最终只存在两个连续的长居地
def del_series_residence_city(city_list, residence_city):
    # 逆序删除
    rst = city_list[:]
    for i in range(len(rst) - 2, 0, -1):
        if rst[i][1] == rst[i - 1][1] and rst[i][1] == rst[i + 1][1] and rst[i][1] == residence_city:
            del rst[i]
    # print(rst)
    return rst


# 3. 按时间间隔分割
# 按时间阈值到列表中将连续两个地点超过阈值的行为分割，此时结果不再记录时间
def separate_city_list_by_time_interval(city_list, time_interval):
    rst = []
    last_time = 0
    tmp_city_route = []
    for i in range(len(city_list)):
        if len(tmp_city_route) == 0:
            tmp_city_route.append(city_list[i][1])
            last_time = city_list[i][0]
            if i == len(city_list) - 1:
                rst.append(tmp_city_route)
            continue
        time_gap_timedelta = city_list[i][0] - last_time
        time_gap = time_gap_timedelta.days
        if time_gap <= time_interval:
            tmp_city_route.append(city_list[i][1])
            last_time = city_list[i][0]
            if i == len(city_list) - 1:
                rst.append(tmp_city_route)
        else:
            rst.append(tmp_city_route)
            if i != len(city_list) - 1:
                tmp_city_route = [city_list[i][1]]
                last_time = city_list[i][0]
    return rst


# 4. 按长居地分割
# 对时间分割的结果按长居地分割一遍，如果出现在时间分割结果的中间，将再次分割
def separate_city_list_by_residence_city(city_list, residence_city):
    rst = []
    for route in city_list:
        if residence_city not in route:
            rst.append(route)
        else:
            tmp_citys = []
            for i in range(0, len(route)):
                tmp_citys.append(route[i])
                if route[i] == residence_city:
                    rst.append(tmp_citys)
                    tmp_citys = []
    return rst


# 5. 对分割后的行为处理
# a. 合并相邻点 b. 删除无效路线（只包含长居地，因为两次长居地的时间可能相近或者超出阈值分成两条路线）
def deal_after_separate(separate_routes, residence_city):
    rst = []
    for route in separate_routes:
        new_route = combine_neighbor(route)  # 合并相邻点
        tmp_new_route = new_route[:]
        for i in range(len(tmp_new_route) - 1, -1, -1):
            if tmp_new_route[i] == residence_city:
                del tmp_new_route[i]
        if len(tmp_new_route) == 0:  # 删除无效路线
            continue
        if new_route[0] != residence_city:  # 补充长居地作为首尾
            new_route.insert(0, residence_city)
        if new_route[len(new_route) - 1] != residence_city:
            new_route.append(residence_city)
        rst.append(new_route)
    return rst


# 从路线列表到字符串
def route_city_list_to_str(route_city_list):
    rst = ''
    for x in route_city_list:
        rst = rst + x + ';'
    return rst


def main_get_city_routes_from_time_city_list(city_list, residence_city, time_interval):
    del_head_tail_rst = del_head_tail_of_city_list(city_list, residence_city)
    del_head_tail_rst = del_error_city(del_head_tail_rst, '不是苏州市')  # 后期更改之后可注释掉
    del_series_residence_user_city_rst = del_series_residence_city(del_head_tail_rst, residence_city)
    time_separate_routes_rst = separate_city_list_by_time_interval(del_series_residence_user_city_rst, time_interval)
    print(time_separate_routes_rst)
    city_separate_routes_rst = separate_city_list_by_residence_city(time_separate_routes_rst, residence_city)
    rst = deal_after_separate(city_separate_routes_rst, residence_city)
    return rst


def city_route_test():
    cursor.execute(
        'SELECT time, cityname FROM suzhou.travel_poi_users_weibodata_suzhou where userid = 36055 order by time')
    user_time_city = cursor.fetchall()
    tmp_users = {36055: []}
    for row in user_time_city:
        tmp_users[36055].append([row[0], row[1]])
    city = '无锡市'
    city_list = tmp_users[36055]
    rst = main_get_city_routes_from_time_city_list(city_list, city, 10)
    print(rst)


def get_insert_sql(user_id, travel_id, route_str, place_num, table_name):
    return "insert into " + table_name + "(userid, travel_id, t_route, place_num) Values({0}, {1}, " \
                                         "\"{2}\", {3});".format(str(user_id), str(travel_id), str(route_str),
                                                                 str(place_num))


# 把用户的城市尺度的路线写入数据库，时间间隔和数据库表名
def main_write_users_city_routes_to_db(time_interval, table_name):
    start_time = time.clock()
    print("时间间隔是 {0} 天，数据库表名是 {1}. ".format(time_interval, table_name))
    uid_city_sql = 'SELECT uid,residence_city FROM suzhou.tourism_id where residence_city is not null;'
    cursor.execute(uid_city_sql)
    user_city_query_rst = cursor.fetchall()
    user_city_dict = {}  # 保存用户 id 和长居地
    for row in user_city_query_rst:
        user_city_dict[row[0]] = row[1]

    done = 0
    count = 0
    n = len(user_city_dict)
    users_city_routes_dict = {}
    for user_id in user_city_dict:
        tmp_user_sql = get_user_time_city_by_id(user_id)
        cursor.execute(tmp_user_sql)
        tmp_user_city_query_rst = cursor.fetchall()
        users_city_routes_dict[user_id] = []
        for row in tmp_user_city_query_rst:
            users_city_routes_dict[user_id].append([row[0], row[1]])
        tmp_user_routes_rst = main_get_city_routes_from_time_city_list(users_city_routes_dict[user_id],
                                                                       user_city_dict[user_id], time_interval)
        for index, route in enumerate(tmp_user_routes_rst):
            route_str = route_city_list_to_str(route)
            insert_sql = get_insert_sql(user_id, index, route_str, len(route) - 2, table_name)
            try:
                cursor.execute(insert_sql)
                db.commit()
            except Exception as e:
                if e.args[0] != 1062:
                    print(e)
        count += 1
        if count / n - done > 0.0001 or count == n:
            done = count / n
            print('写入完成度：%.2f%%    %f s' % ((done * 100), time.clock() - start_time), end='\r')
            # print('写入完成度：%.2f%%    %f' % ((done * 100), time.clock() - s))
    print('写入完成度：100%')


# 构建motif的网络，写入文件中，返回简单路径集合
def main_generate_city_motif_network(routes_sql, network_save_file_name):
    print('构建 motif 的网络...')
    cursor.execute(routes_sql)
    data = cursor.fetchall()
    count = len(data)
    simple_routes = []
    for x in range(0, count):
        tmp = data[x][0][:-1]
        simple_route = raw_to_simple_route(tmp)
        if len(simple_route) > 1:
            simple_routes.append(simple_route)  # 从原始路线数据转成精简路径，合并相邻点，去掉合并后只有一个点的行为，所以路线中可能有3个地点却会保留5个边

    edges, place_num = network(simple_routes)  # 生成网络边
    with open(network_save_file_name, 'w') as f:  # 将有向边写入文件 network.txt 中，然后使用 FanMod/netmode 进行 motif 发现
        f.write(str(place_num) + '\n')
        for edge in edges:
            edge_str = str(edge[0]) + ' ' + str(edge[1])
            f.write(edge_str + "\n")
    return simple_routes


def city_routes_test(user_id, time_interval):
    uid_city_sql = 'SELECT uid,residence_city FROM suzhou.tourism_id where uid = ' + str(user_id)
    cursor.execute(uid_city_sql)
    user_city_query_rst = cursor.fetchall()
    user_city_dict = {}  # 保存用户 id 和长居地
    for row in user_city_query_rst:
        user_city_dict[row[0]] = row[1]

    tmp_user_sql = get_user_time_city_by_id(user_id)
    cursor.execute(tmp_user_sql)
    tmp_user_city_query_rst = cursor.fetchall()
    users_city_routes_dict = {}
    users_city_routes_dict[user_id] = []
    for row in tmp_user_city_query_rst:
        users_city_routes_dict[user_id].append([row[0], row[1]])
    tmp_user_routes_rst = main_get_city_routes_from_time_city_list(users_city_routes_dict[user_id],
                                                                   user_city_dict[user_id], time_interval)
    print(tmp_user_routes_rst)


# 从数据库获取城市距离字典, sql 返回三列数据，城市a， 城市b，距离s
def load_city_distance_dict_from_db(sql, db_cursor):
    db_cursor.execute(sql)
    data = db_cursor.fetchall()
    distance_dict = {}
    for row in data:
        if (row[0], row[1]) in distance_dict or (row[1], row[0]) in distance_dict:
            continue
        else:
            distance_dict[(row[0], row[1])] = row[2]
    return distance_dict


def write_route_distance_to_db(db_cursor):
    city_dis_dict = load_city_distance_dict_from_db(const.city_distance_sql, db_cursor)
    rst = []
    sql = 'SELECT t_route, userid, travel_id FROM suzhou.travel_users_city_routes_3 where place_num > 0 and place_num < 7;'
    db_cursor.execute(sql)
    route_str_data = db_cursor.fetchall()
    for route_str_tuple in route_str_data:
        route_str = route_str_tuple[0]
        simple_route = raw_to_simple_route(route_str)
        if simple_route:
            rst.append((simple_route, route_str_tuple[1], route_str_tuple[2]))
    count = 0
    for x in rst:
        dis = tools.calculate_distance(x[0], city_dis_dict)
        update_sql = 'update suzhou.travel_users_city_routes_3 set distance = {0} where userid = {1} and travel_id = {2};'.format(
            dis, x[1], x[2])
        db_cursor.execute(update_sql)
        count += 1
        if count % 1000 == 0:
            print(count * 100 / len(rst), '%')
    print('add distance finish.')


def new_k_mean(routes, k, model_dis_dict):
    old_k_center = []
    seed_center = {}

    while True:
        tmp_int = random.randint(0, len(routes))
        seed = ' '.join(routes[tmp_int][:])
        if seed not in seed_center:
            seed_center[seed] = 1
        if len(seed_center) == k:
            break
    # 获取开始的 k 个种子点
    for seed in seed_center:
        old_k_center.append(seed.split(' '))
    count = 1
    print('正在进行第 ' + str(count) + ' 次迭代')

    match_routes = []
    for x in range(0, k):
        match_routes.append([])
    for route in routes:
        index = find_nearest_center(old_k_center, route)
        match_routes[index].append(route)
    new_seed_center = []
    print('匹配完成，寻找新种子点')
    for i, x in enumerate(match_routes):
        new_seed, model_dis_dict = calculate_center_by_model_dict(x, model_dis_dict)
        new_seed_center.append(new_seed)
    print("新中心：" + str(new_seed_center))
    is_done = not operator.eq(new_seed_center, old_k_center)
    old_centers = []
    while is_done:
        count += 1
        print('正在进行第 ' + str(count) + ' 次迭代')
        old_centers.append(old_k_center[:])
        old_k_center = new_seed_center
        new_seed_center = []
        match_routes = []
        for x in range(0, k):
            match_routes.append([])
        for route in routes:
            index = find_nearest_center(old_k_center, route)
            match_routes[index].append(route)
        for i, x in enumerate(match_routes):
            new_seed, model_dis_dict = calculate_center_by_model_dict(x, model_dis_dict)
            new_seed_center.append(new_seed)
        print("新中心：" + str(new_seed_center))
        if operator.eq(new_seed_center, old_k_center):
            print('种子点不变化，结束程序')
            is_done = False
        else:
            for center in old_centers:
                if operator.eq(new_seed_center, center):
                    is_done = False
                    print('进入循环，退出')
                    break

    write_model_dis_dict_to_file(model_dis_dict, model_dis_dict_filename)
    tmp = get_filename_time(model_dis_dict_filename)
    write_model_dis_dict_to_file(model_dis_dict, tmp)
    print('共进行了 ' + str(count) + ' 次迭代')
    print('新种子', new_seed_center)
    for i in range(1, k + 1):
        print(i - 1, match_routes[i - 1])
    return new_seed_center, match_routes


# all centers not change
def new_k_mean_center(routes, k, model_dis_dict):
    old_k_center = []
    seed_center = {}

    while True:
        tmp_int = random.randint(0, len(routes))
        seed = ' '.join(routes[tmp_int][:])
        if seed not in seed_center:
            seed_center[seed] = 1
        if len(seed_center) == k:
            break
    # 获取开始的 k 个种子点
    for seed in seed_center:
        old_k_center.append(seed.split(' '))
    count = 1
    print('正在进行第 ' + str(count) + ' 次迭代')

    match_routes = []
    for x in range(0, k):
        match_routes.append([])
    for route in routes:
        index = find_nearest_center(old_k_center, route)
        match_routes[index].append(route)
    new_seed_center = []
    print('匹配完成，寻找新种子点')
    for i, x in enumerate(match_routes):
        new_seed, model_dis_dict = calculate_center_by_model_dict(x, model_dis_dict)
        new_seed_center.append(new_seed)
    print("新中心：" + str(new_seed_center))
    is_done = not operator.eq(new_seed_center, old_k_center)
    old_centers = []
    while is_done:
        count += 1
        print('正在进行第 ' + str(count) + ' 次迭代')
        old_centers.append(old_k_center[:])
        old_k_center = new_seed_center
        new_seed_center = []
        match_routes = []
        for x in range(0, k):
            match_routes.append([])
        for route in routes:
            index = find_nearest_center(old_k_center, route)
            match_routes[index].append(route)
        for i, x in enumerate(match_routes):
            new_seed, model_dis_dict = calculate_center_by_model_dict(x, model_dis_dict)
            new_seed_center.append(new_seed)
        print("新中心：" + str(new_seed_center))
        if operator.eq(new_seed_center, old_k_center):
            print('种子点不变化，结束程序')
            is_done = False
        else:
            for center in old_centers:
                if operator.eq(new_seed_center, center):
                    is_done = False
                    print('进入循环，退出')
                    break

    write_model_dis_dict_to_file(model_dis_dict, model_dis_dict_filename)
    tmp = get_filename_time(model_dis_dict_filename)
    write_model_dis_dict_to_file(model_dis_dict, tmp)
    print('共进行了 ' + str(count) + ' 次迭代')
    print('新种子', new_seed_center)
    for i in range(1, k + 1):
        print(i - 1, match_routes[i - 1])
    return new_seed_center, match_routes


# 使用向量求距离
def vector_k_mean(routes, k):

    def get_place_dict(routes):
        index = 0
        rst = {}
        for route in routes:
            for poi in route:
                if poi not in rst:
                    rst[poi] = index
                    index += 1
        return rst

    place_dict = get_place_dict(routes)

    def get_route_vector(route):
        rst = [0] * len(place_dict)
        for poi in route:
            rst[place_dict[poi]] = 1
        return rst

    def get_dis(route1, route2):
        v1 = get_route_vector(route1)
        v2 = get_route_vector(route2)
        sum = 0
        for i in range(len(v1)):
            sum = sum + (v2[i] - v1[i]) * (v2[i] - v1[i])
        return math.sqrt(sum)

    def choose_center(centers, route):
        dis_list = []
        for center in centers:
            dis = get_dis(center, route)
            dis_list.append(dis)
        index = dis_list.index(min(dis_list))
        return index

    def find_new_center(routes):
        if len(routes) == 0:
            return []
        dis_list = []
        for route in routes:
            sum = 0
            for other in routes:
                dis = get_dis(route, other)
                sum += dis
            dis_list.append(sum)
        # print(dis_list)
        index = dis_list.index(min(dis_list))
        return routes[index]

    def SSE(k, centers, groups):
        rst = 0
        for i in range(k):
            sum = 0
            for route in groups[i]:
                sum += get_dis(route, centers[i])
            rst += sum
        return rst

    old_k_center = []
    seed_center = {}

    while True:
        tmp_int = random.randint(0, len(routes) - 1)
        seed = ' '.join(routes[tmp_int][:])
        if seed not in seed_center:
            seed_center[seed] = 1
        if len(seed_center) == k:
            break
    # 获取开始的 k 个种子点
    for seed in seed_center:
        old_k_center.append(seed.split(' '))

    # old_k_center = [['拙政园', '苏州博物馆', '平江路', '金鸡湖'], ['寒山寺', '虎丘山', '山塘'], ['狮子林', '拙政园', '观前街']]
    # old_k_center = [['平江路', '拙政园', '观前街', '金鸡湖'], ['观前街', '拙政园', '苏州博物馆', '山塘'], ['周庄', '寒山寺', '观前街', '拙政园'], ['周庄', '同里', '平江路', '观前街']]
    # [['平江路', '观前街', '金鸡湖'], ['寒山寺', '虎丘山', '山塘'], ['狮子林', '拙政园', '观前街']]
    # ['平江路', '拙政园', '观前街', '金鸡湖'], ['山塘', '观前街', '拙政园', '寒山寺'], ['光福', '木渎', '金鸡湖', '西山']
    match_routes = []
    for x in range(0, k):
        match_routes.append([])

    for route in routes:
        index = choose_center(old_k_center, route)
        match_routes[index].append(route)
    new_seed_center = []

    print('匹配完成，寻找新种子点')
    for group in match_routes:
        new_seed = find_new_center(group)
        new_seed_center.append(new_seed)

    print("新中心：" + str(new_seed_center))
    is_done = not operator.eq(new_seed_center, old_k_center)
    old_centers = []
    count = 0
    while is_done:
        count += 1
        print('正在进行第 ' + str(count) + ' 次迭代')
        old_centers.append(old_k_center[:])
        old_k_center = new_seed_center
        new_seed_center = []
        match_routes = []
        for x in range(0, k):
            match_routes.append([])
        # 重新匹配中心
        for route in routes:
            index = choose_center(old_k_center, route)
            match_routes[index].append(route)
        # 重新计算中心
        for group in match_routes:
            new_seed = find_new_center(group)
            new_seed_center.append(new_seed)
        print("新中心：" + str(new_seed_center))
        # 判断结束
        if operator.eq(new_seed_center, old_k_center):
            print('种子点不变化，结束程序')
            is_done = False
        else:
            for center in old_centers:
                if operator.eq(new_seed_center, center):
                    is_done = False
                    print('进入循环，退出')
                    break
    # 结束
    # print('共进行了 ' + str(count) + ' 次迭代')
    # print('新种子', SSE(k, new_seed_center, match_routes), new_seed_center)
    # for i in range(1, k + 1):
    #     print(i - 1, len(match_routes[i - 1]), match_routes[i - 1])
    return SSE(k, new_seed_center, match_routes), new_seed_center, match_routes
