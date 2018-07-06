import math
import re
from typing import Dict, Any

import networkx as nx
import numpy as np
import pylab as pl

import const

error_num = 987654321


# 从矩阵字符串转换成 数字矩阵 n*n
def parse_str_to_matrix(matrix_str):
    '''
    从矩阵字符串转换成 数字矩阵 n*n
    :param matrix_str: 矩阵字符串形式
    :return: 数字矩阵
    >>> parse_str_to_matrix('010110101')
    [[0, 1, 0], [1, 1, 0], [1, 0, 1]]
    '''
    n = int(math.sqrt(len(matrix_str)))
    matrix = []
    row = []
    for i in range(n):
        for j in range(n):
            row.append(int(matrix_str[i * n + j]))
        matrix.append(row)
        row = []
    return matrix


# 从 n*n 的图矩阵转为字符串形式
def parse_matrix_to_str(matrix):
    '''
    从 n*n 的图矩阵转为字符串形式
    :param matrix: n*n 的图数字矩阵
    :return: 图的矩阵字符串表示
    >>> parse_matrix_to_str([[0, 1, 0], [1, 1, 0], [1, 0, 1]])
    '010110101'
    '''
    rst = ''
    for line in matrix:
        for x in line:
            rst += str(x)
    return rst


# 从矩阵字符串画图，是否需要显示标号
def draw_image_from_matrix_str(matrix_str, with_labels):
    '''
    从矩阵字符串画图，是否需要显示标号
    :param matrix_str: 矩阵字符串
    :param with_labels: 是否显示标号
    :return:
    '''
    mat_l = parse_str_to_matrix(matrix_str)
    mat = np.array(mat_l)
    g = nx.from_numpy_matrix(mat, create_using=nx.DiGraph())
    nx.draw(g, with_labels=with_labels)
    pl.show()


# 由十进制数字转成指定节点的图的矩阵字符串
def num_to_motif_matrix(num, motif_size):
    '''
    从矩阵字符串画图，是否需要显示标号
    :param num: 图的标号（十进制）
    :param motif_size: 要转成图的节点数目
    :return: 矩阵字符串
    >>> num_to_motif_matrix(6, 3)
    '000000110'
    >>> num_to_motif_matrix(8729928514, 6)
    '001000001000010110000010001101000010'
    '''
    bin_num = bin(num)[2:]
    length = motif_size * motif_size
    if length > len(bin_num):
        tmp = ['0'] * (length - len(bin_num))
        head = ''.join(tmp)
        bin_num = head + bin_num
    return bin_num


def motif_matrix_to_num(matrix_str):
    n = int(math.sqrt(len(matrix_str)))
    return int(matrix_str, 2)

# 从 netmode 结果文件中一行 motif 信息提取出结果字典
def get_graph_data_from_file_line(line):
    '''
    从 netmode 结果文件中一行 motif 信息提取出结果字典
    :param line: netmode 结果文件中的一行
    :return: 结果字典
    '''
    pattern = re.compile(r'\S+: +\S+')
    result = pattern.findall(line)
    rst = {}
    for item in result:
        if item[0] == 'g':
            rst[item.split(':')[0]] = int(item.split(':')[1])
        elif item[0] == '(':
            continue
        else:
            try:
                rst[item.split(':')[0]] = float(item.split(':')[1])
            except Exception as e:
                rst[item.split(':')[0]] = error_num
            else:
                pass
            finally:
                pass

    return rst


# 从 netmode 结果文件中筛选所要的 motifs
def read_motifs_from_file(filename, size, min_z_score, max_p_value):
    '''
    从 netmode 结果文件中筛选所要的 motifs
    :param filename: 文件名
    :param size: 节点大小
    :param min_z_score: 最小 Z-Score 阈值
    :param max_p_value: 最大 p-value 阈值
    :return: 符合条件的 motif 集合
    '''
    rst = []
    with open(filename, 'r') as f:
        while True:
            line = f.readline()
            if len(line) == 0:
                break
            if line[0] != 'g':
                continue
            tmp_motif = get_graph_data_from_file_line(line)
            # if tmp_motif['f-ZScore'] != error_num:
            #     if abs(tmp_motif['f-ZScore']) >= min_z_score and tmp_motif['f-pValue'] <= max_p_value:
            #         rst.append(num_to_motif_matrix(tmp_motif['gID'], size))
            # else:
            #     if abs(tmp_motif['c-ZScore']) >= min_z_score and tmp_motif['c-pValue'] <= max_p_value:
            #         rst.append(num_to_motif_matrix(tmp_motif['gID'], size))
            fzscore, czscore, fpvalue, cpvalue = abs(tmp_motif['f-ZScore']), abs(tmp_motif['c-ZScore']), tmp_motif['f-pValue'], tmp_motif['c-pValue']
            zscore = 0
            if fzscore != error_num and czscore != error_num:
                zscore = max(abs(fzscore), abs(czscore))
            elif fzscore == error_num and czscore != error_num:
                zscore = abs(czscore)
            elif fzscore != error_num and czscore == error_num:
                zscore = abs(fzscore)
            else:
                print('error in selecting motif, gID = ', tmp_motif['gID'])
            if zscore >= min_z_score:
                rst.append(num_to_motif_matrix(tmp_motif['gID'], size))
                continue
            pvalue = min(fpvalue, cpvalue)
            if pvalue <= max_p_value:
                rst.append(num_to_motif_matrix(tmp_motif['gID'], size))
                continue
            if abs(fpvalue - cpvalue) > 0.1 * pvalue:   # 两个 p-value 差别太大时，将其选中
                rst.append(num_to_motif_matrix(tmp_motif['gID'], size))
                continue

    return rst

def read_motif_data_from_file(filename, id_list):
    rst = []
    with open(filename, 'r') as f:
        while True:
            line = f.readline()
            if len(line) == 0:
                break
            if line[0] != 'g':
                continue
            tmp_motif = get_graph_data_from_file_line(line)
            if tmp_motif['gID'] in id_list:
                rst.append(tmp_motif)
    return rst


# 合并不同节点的motif
def combine_motifs_list(*diff_size_motifs):
    rst = []
    for motifs in diff_size_motifs:
        for element in motifs:
            rst.append(element)
    return rst


def check_city_func(route):
    '''
    判断城市的职能
    0: 客源地
    1: 单目的地
    2: 门户
    3: 出口
    4: 枢纽
    5: 途经
    :param route: 路线
    :return: 每个节点的作用的结合
    >>> check_city_func(['上海', '北京', '上海'])
    {'上海': 0, '北京': 1}
    >>> check_city_func(['上海', '北京', '广州', '北京', '上海'])
    {'上海': 0, '北京': 4, '广州': 5}
    '''
    places = {}
    num_to_place = {}
    num = 0
    for x in route:
        if x not in places:
            places[x] = num
            num_to_place[num] = x
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
    route_matrix_str = ''.join(mat)
    rst: Dict[Any, int] = {num_to_place[0]: 0}
    if route_matrix_str in const.ring_and_chain:
        return None
    else:
        route_matrix = parse_str_to_matrix(route_matrix_str)
        if route_matrix_str not in const.city_order_motif_gt_300:
            return None
        for x in places:
            index = places[x]
            if index == 0:
                continue
            else:
                if route_matrix[0][index] == 1 and route_matrix[index][0] == 1:
                    rst[x] = 4
                elif route_matrix[0][index] == 1:
                    rst[x] = 2
                elif route_matrix[index][0] == 1:
                    rst[x] = 3
                else:
                    rst[x] = 5
        return rst
    # if route_matrix_str == '0110':  # 单目的地
    #     rst[num_to_place[1]] = 1
    #     return rst
    # else:
    #     route_matrix = parse_str_to_matrix(route_matrix_str)
    #     if route_matrix_str not in const.city_order_motif_gt_300:
    #         return None
    #     for x in places:
    #         index = places[x]
    #         if index == 0:
    #             continue
    #         else:
    #             if route_matrix[0][index] == 1 and route_matrix[index][0] == 1:
    #                 rst[x] = 4
    #             elif route_matrix[0][index] == 1:
    #                 rst[x] = 2
    #             elif route_matrix[index][0] == 1:
    #                 rst[x] = 3
    #             else:
    #                 rst[x] = 5
    # return rst


def calculate_distance(route, city_distance_dict):
    dis = 0
    for i in range(len(route) - 1):
        if (route[i], route[i + 1]) in city_distance_dict:
            dis += city_distance_dict[(route[i], route[i + 1])]
        else:
            dis += city_distance_dict[(route[i + 1], route[i])]
    return dis



if __name__ == '__main__':
    import doctest

    doctest.testmod(verbose=False)
