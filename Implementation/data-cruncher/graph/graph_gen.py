import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import csv
import sys

def get_csv_data(file_path):
    f = open(file_path, "r")
    reader = csv.reader(f)
    reader.next()
    i = 0
    data = []
    for row in reader:
        item = row[1].strip()
        item = float(item)
        data.append(item)
        i = i + 1
        if i == 50:
            break
    f.close()
    return data


def setup_graph(ax, x_label, y_label, title):
    ax.set_xlabel(x_label, fontsize=12)
    ax.set_ylabel(y_label, fontsize=12)
    ax.set_title(title, fontsize=12)

def check_file(path):
    if path == "":
        raise ValueError("path cannot be empty")


if __name__ == "__main__":
    if len(sys.argv) < 4:
        print "Need to pass two arguments"
        sys.exit()
    CSV_FILE_1 = sys.argv[1]
    check_file(CSV_FILE_1)
    CSV_FILE_2 = sys.argv[2]
    check_file(CSV_FILE_2)
    graph_name = sys.argv[3]
    fig, ax = plt.subplots()
    setup_graph(ax, "Iteration", "Min Interference", graph_name)
    data = get_csv_data(CSV_FILE_1)
    s1 = ax.plot([i for i in range(len(data))], data, c='b', label="Method 1")
    data = get_csv_data(CSV_FILE_2)
    s2 = ax.plot([i for i in range(len(data))], data, c='g', label = "Method 2")

    plt.legend()

    plt.grid(True)

    plt.savefig("graph/"+graph_name)
