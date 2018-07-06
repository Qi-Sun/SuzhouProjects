import math

import tools


# 利用并查集，通过矩阵字符串判断该有向图是否是连通的
def judge_graph_connected(origin_matrix_str):
    '''
    利用并查集，通过矩阵字符串判断该有向图是否是连通的
    :param origin_matrix_str: 有向图是否连通
    :return: 是否连通
    '''
    pre = {}

    def find(x):
        r = x
        while pre[r] != r:
            r = pre[r]
        i = x
        while i != r:       # 路径压缩，平衡树的层次
            j = pre[i]
            pre[i] = j
            i = j
        return r

    def join(x, y):
        fx = find(x)
        fy = find(y)
        if fx != fy:
            root = min(fx, fy)  # 平衡树的层次
            pre[fx] = root
            pre[fy] = root

    def judge(n, edges):
        '''
        判断是否连通
        :param n: 节点数
        :param edges: 边的集合
        :return: 是否连通
        >>> judge(4, [(0, 1), (2, 0),(2, 3)])
        True
        '''
        for i in range(n):
            pre[i] = i
        for i in range(len(edges)):
            join(edges[i][0], edges[i][1])
        group = 0
        for i in range(n):
            if pre[i] == i:
                group += 1
        if group == 1:
            return True
        else:
            return False

    def get_matrix_edges_for_connected_judge(matrix_str):
        matrix = tools.parse_str_to_matrix(matrix_str)
        rst_dict = {}
        n = int(math.sqrt(len(matrix_str)))
        for i in range(n):
            for j in range(n):
                if matrix[i][j] == 1:
                    tmp_key = str(i) + ' ' + str(j)
                    tmp_key_1 = str(j) + ' ' + str(i)
                    if tmp_key not in rst_dict and tmp_key_1 not in rst_dict:
                        rst_dict[tmp_key] = 1
        rst = []
        for x in rst_dict:
            rst.append((int(x.split(' ')[0]), int(x.split(' ')[1])))
        return rst

    size = int(math.sqrt(len(origin_matrix_str)))
    graph_edges = get_matrix_edges_for_connected_judge(origin_matrix_str)
    return judge(size, graph_edges)


# 从矩阵字符串获取每个点的进度和出度
def get_degree_from_str(matrix_str):
    matrix = tools.parse_str_to_matrix(matrix_str)
    n = len(matrix)
    degree_rst = []
    for i in range(0, n):
        in_degree = 0
        out_degree = 0
        for j in range(0, n):
            if matrix[i][j] == 1:
                out_degree += 1
        for j in range(0, n):
            if matrix[j][i] == 1:
                in_degree += 1
        degree_rst.append([in_degree, out_degree])
    return degree_rst


# 给定邻接矩阵的字符串表示，判断是否存在欧拉环
def is_euler_ring(matrix_str):
    '''
    判断邻接矩阵的途中是否由又拉回路
    :param matrix_str: 矩阵字符串
    :return: 是否存在欧拉回路
    >>> is_euler_ring('010001100')
    True
    >>> is_euler_ring('0001100101010011000111110')
    True
    >>> is_euler_ring('010001010')
    False
    >>> is_euler_ring('010001110')
    False
    '''
    if not judge_graph_connected(matrix_str):
        return False
    degree_rst = get_degree_from_str(matrix_str)
    for x in degree_rst:
        if x[0] != x[1]:
            return False
    return True


# 给定邻接矩阵的字符串表示，判断是否存在欧拉路径
def is_euler_path(matrix_str):
    '''
    判断图中是否由欧拉路径
    :param matrix_str:
    :return: 是否有欧拉路径
    >>> is_euler_path('0001100101010011000111110')
    True
    >>> is_euler_path('010001010')
    True
    >>> is_euler_path('010001110')
    True
    '''
    if not judge_graph_connected(matrix_str):
        return False
    rst = is_euler_ring(matrix_str)
    if rst:
        return True
    degree_rst = get_degree_from_str(matrix_str)
    tmp_degree = []
    for x in degree_rst:
        if x[0] != x[1]:
            tmp_degree.append(x)
    if len(tmp_degree) != 2:
        return False
    in_more_than_out = 0
    out_more_than_in = 0
    for x in tmp_degree:
        if x[0] == x[1] + 1:
            in_more_than_out += 1
        elif x[0] == x[1] - 1:
            out_more_than_in += 1
    if in_more_than_out == 1 and out_more_than_in == 1:
        return True
    return False


# 是否有欧拉路径（回路）
def have_euler_path(matrix_str):
    euler_path = is_euler_path(matrix_str)
    if euler_path:
        return True
    return False


# 判断途中是否有指定出入度的顶点存在，用于判断是否能作为长居地
def count_in_out_degree_num(matrix_str, in_degree, out_degree):
    degree_rst = get_degree_from_str(matrix_str)
    count = 0
    for x in degree_rst:
        if x[0] == in_degree and x[1] == out_degree:
            count += 1
    if count == 0:
        return False
    else:
        return True


# 从矩阵字符串中找出指定点的临近点集合
def get_adjs(matrix_str, v):
    '''
    从矩阵字符串中找出指定点的临近点集合
    :param matrix_str: 矩阵字符串
    :param v: 指定点的位置，index
    :return: 所有临近点的结合，即以 v 为起点，由以这些点为终点的边
    >>> get_adjs('001000001000010110000010001101000010', 0)
    [2]
    >>> get_adjs('001000001000010110000010001101000010', 2)
    [1, 3, 4]
    '''
    n = int(math.sqrt(len(matrix_str)))
    rst = []
    for i in range(n):
        if matrix_str[v * n + i] == '1':
            rst.append(i)
    return rst


# 从矩阵字符串和指定点获取其该点为起点的所有欧拉路径（包括回路）的顶点序列集合的集合
def get_all_euler_paths(origin_matrix_str, first_node):
    '''
    从矩阵字符串和指定点获取其该点为起点的所有欧拉路径（包括回路）的顶点序列集合的集合
    :param origin_matrix_str: 矩阵字符串
    :param first_node: 指定起始点
    :return: 以其为起始点的所有欧拉路径（回路）的顶点序列列表的集合
    >>> get_all_euler_paths('010001000', 0)
    [[0, 1, 2]]
    >>> get_all_euler_paths('010001100', 0)
    [[0, 1, 2, 0]]
    >>> get_all_euler_paths('010101010', 0)
    [[0, 1, 2, 1, 0]]
    '''
    # 重新判断一次是否连通
    if not have_euler_path(origin_matrix_str):
        return []
    all_path = []
    origin_matrix = tools.parse_str_to_matrix(origin_matrix_str)

    def dfs(matrix, path, start_node):
        path.append(start_node)
        matrix_str = tools.parse_matrix_to_str(matrix)
        adj_nodes = get_adjs(matrix_str, start_node)
        for node in adj_nodes:
            matrix[start_node][node] = 0
            dfs(matrix, path, node)
        done = True
        path_size = len(path)
        for line in matrix:
            if 1 in line:
                matrix[path[path_size - 2]][path[path_size - 1]] = 1  # 如果最终还有边没遍历到，失败，返回上层尝试
                path.pop()
                done = False
                break
        if done:  # 最终已经遍历所有边，成功，保存，返回上层尝试
            all_path.append(path[:])
            matrix[path[path_size - 2]][path[path_size - 1]] = 1
            path.pop()

    dfs(origin_matrix, [], first_node)
    return all_path


# 从图中的欧拉路径重新对顶点排序
def transfer_vertex_from_euler_path(matrix_str, euler_path):
    '''
    从图中的欧拉路径重新对顶点排序
    :param matrix_str: 矩阵字符串
    :param euler_path: 欧拉路径顶点序列（提供重新排序的顺序）
    :return: 排序后的矩阵的字符串
    >>> transfer_vertex_from_euler_path('001000010', [0, 2, 1])
    '010001000'
    >>> transfer_vertex_from_euler_path('0000100010000110110010100', [0, 4, 2, 3, 1, 3, 2, 4, 0])
    '0100010100010100010100010'
    >>> transfer_vertex_from_euler_path('0000100010000110110010100', [1, 3, 2, 4, 0, 4, 2, 3, 1])
    '0100010100010100010100010'
    '''
    new_order = []
    n = int(math.sqrt(len(matrix_str)))
    for i in euler_path:
        if i not in new_order:
            new_order.append(i)
        if len(new_order) == n:
            break
    order_dict = {}
    for i in range(n):
        order_dict[new_order[i]] = i    # 获得新旧顶点对应关系
    new_matrix = []
    origin_matrix = tools.parse_str_to_matrix(matrix_str)
    for i in new_order:
        new_line = [0] * n
        for j in range(n):
            if origin_matrix[i][j] == 1:
                new_line[order_dict[j]] = 1
        new_matrix.append(new_line)
    rst = tools.parse_matrix_to_str(new_matrix)
    return rst


# 获取整个矩阵的所有同构的矩阵字符串，所有顶点遍历获取
def get_all_euler_path_matrix_str(matrix_str):
    '''
    获取整个矩阵的所有同构的矩阵字符串
    :param matrix_str: 矩阵字符串
    :return: 所有同构的矩阵字符串集合
    '''
    n = int(math.sqrt(len(matrix_str)))
    euler_paths = []
    for i in range(n):      # 获取所有点为起点的欧拉路径
        tmp_euler_paths = get_all_euler_paths(matrix_str, i)
        for path in tmp_euler_paths:
            euler_paths.append(path)
    # 重新排序所有顶点
    rst = []
    for path in euler_paths:
        tmp_matrix_str = transfer_vertex_from_euler_path(matrix_str, path)
        if tmp_matrix_str not in rst:
            rst.append(tmp_matrix_str)
    return rst


# 找出所有欧拉路径
def get_all_euler_path_by_matrix_str_by_node_degree(matrix_str):
    '''
    指定开始顶点的出度和入度，以其为起点找出所有欧拉路径后重排序的同构图的字符串
    :param matrix_str: 图的字符串
    :return: 所有符合条件的同构图的矩阵字符串集合
    >>> get_all_euler_path_by_matrix_str_by_node_degree('0001001001011010')
    ['0100101001010010']
    '''
    if not have_euler_path(matrix_str):       # 如果不存在欧拉路径，不再继续
        return []
    n = int(math.sqrt(len(matrix_str)))
    degree_rst = get_degree_from_str(matrix_str)    # (in_degree, out_degree)
    suit_node = []
    for i in range(n):
        # 出入度相等或者初出度比入度多 1
        if degree_rst[i][0] == degree_rst[i][1] or degree_rst[i][0] + 1 == degree_rst[i][1]:
            suit_node.append(i)

    euler_paths = []
    for i in suit_node:
        tmp_euler_paths = get_all_euler_paths(matrix_str, i)
        for path in tmp_euler_paths:
            euler_paths.append(path)
        # 重新排序所有顶点
    rst = []
    for path in euler_paths:
        tmp_matrix_str = transfer_vertex_from_euler_path(matrix_str, path)
        if tmp_matrix_str not in rst:
            rst.append(tmp_matrix_str)
    return rst



# 指定开始顶点的出度和入度，以其为起点找出所有欧拉图后，重排序的同构图的字符串，且要求欧拉图的起点和终点是同一个点
def get_all_euler_ring_by_matrix_str_by_node_degree(matrix_str, in_degree, out_degree):
    '''
    指定开始顶点的出度和入度，以其为起点找出所有欧拉路径后重排序的同构图的字符串
    :param matrix_str: 图的字符串
    :param in_degree: 顶点入度
    :param out_degree: 顶点出度
    :return: 所有符合条件的同构图的矩阵字符串集合
    >>> get_all_euler_path_by_matrix_str_by_node_degree('0001001001011010', 1, 1)
    ['0100101001010010']
    '''
    if not is_euler_ring(matrix_str):       # 如果不存在 欧拉图 不再继续
        return []
    n = int(math.sqrt(len(matrix_str)))
    degree_rst = get_degree_from_str(matrix_str)
    suit_node = []
    for i in range(n):
        if degree_rst[i][0] == in_degree and degree_rst[i][1] == out_degree:
            suit_node.append(i)
    euler_paths = []
    for i in suit_node:
        tmp_euler_paths = get_all_euler_paths(matrix_str, i)
        # if matrix_str == '11000100010000010000010000101000010':
        #     print(tmp_euler_paths)
        #     tools.draw_image_from_matrix_str(matrix_str, True)
        for path in tmp_euler_paths:
            euler_paths.append(path)
        # 重新排序所有顶点
    rst = []
    # print(euler_paths)
    for path in euler_paths:
        tmp_matrix_str = transfer_vertex_from_euler_path(matrix_str, path)
        if tmp_matrix_str not in rst:
            rst.append(tmp_matrix_str)
    return rst

if __name__ == '__main__':
    import doctest

    doctest.testmod()
