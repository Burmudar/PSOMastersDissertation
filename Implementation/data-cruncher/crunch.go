package main

import (
	"bufio"
	"encoding/csv"
	"fmt"
	"log"
	"os"
	"path/filepath"
	"strconv"
	"strings"
)

const (
	BENCHMARK_PREFIX = "Benchmark-Bench-"
	KEY_SEP          = "-"
)

type benchmark struct {
	File           string
	Name           string
	MoveFuncName   string
	BuildStratName string
	CostFuncName   string
	Population     int
	StdDev         float32
	Avg            float32
	Min            float32
	Max            float32
	Variance       float32
	SumOfDiff      float32
}

func (b *benchmark) Key() string {
	return b.Name + KEY_SEP + b.MoveFuncName +
		KEY_SEP + b.BuildStratName + KEY_SEP +
		b.CostFuncName + KEY_SEP + strconv.Itoa(b.Population)
}

func trimAndToFloat(val string) float32 {
	val = strings.Trim(val, " ")
	parts := strings.Split(val, " ")
	fVal, error := strconv.ParseFloat(parts[1], 32)
	if error != nil {
		log.Fatal(error)
	}
	return float32(fVal)
}

func (b *benchmark) LoadLastValues() {
	fileReader := openFile(b.File)
	reader := csv.NewReader(fileReader)
	reader.FieldsPerRecord = 2
	records, error := reader.ReadAll()
	if error != nil {
		log.Fatal(error)
	}
	b.StdDev = trimAndToFloat(records[51][0])    //std dev
	b.Avg = trimAndToFloat(records[51][1])       //avg
	b.Min = trimAndToFloat(records[52][0])       //min
	b.Max = trimAndToFloat(records[52][1])       //max
	b.Variance = trimAndToFloat(records[53][0])  //variance
	b.SumOfDiff = trimAndToFloat(records[53][1]) //SumOfDiff
}

func stripDir(fullname string) (dir, rest string) {
	slashPos := strings.Index(fullname, "/")
	if slashPos == -1 {
		return "", fullname
	} else {
		dir = fullname[0 : slashPos+1]
		rest = fullname[slashPos+1 : len(fullname)]
		return
	}
}

func stripNameForBenchmark(fullname string) (name, rest string) {
	rest = strings.TrimPrefix(fullname, BENCHMARK_PREFIX)
	bracket := strings.Index(rest, "(")
	name = rest[0:bracket]
	return name, strings.TrimPrefix(rest, name)
}

func stripValueBetweenBrackets(portion, openBracket, closeBracket string) (name, rest string) {
	rest = portion[strings.Index(portion, openBracket)+1 : len(portion)] //get rid of first bracket
	closeBracketPos := strings.Index(rest, closeBracket)
	name = rest[0:closeBracketPos]
	return name, rest[closeBracketPos+1 : len(rest)]
}

func NewBenchmark(file *string) *benchmark {
	b := new(benchmark)
	b.File = *file
	_, filename := stripDir(b.File)
	fmt.Println(filename)
	name, rest := stripNameForBenchmark(filename)
	b.Name = name
	b.MoveFuncName, rest = stripValueBetweenBrackets(rest, "(", ")")
	b.BuildStratName, rest = stripValueBetweenBrackets(rest, "(", ")")
	b.CostFuncName, rest = stripValueBetweenBrackets(rest, "[", "]")
	pop, rest := stripValueBetweenBrackets(rest, "(", ")")
	popNum, error := strconv.Atoi(pop)
	if error != nil {
		log.Fatal("Could not convert" + pop + " to a number")
	} else {
		b.Population = popNum
	}
	fmt.Println(rest)
	return b
}

func openFile(filename string) *bufio.Reader {
	file, err := os.Open(filename)
	if err != nil {
		log.Fatal(err)
	}
	return bufio.NewReader(file)
}

func GetCSVFiles(dirPath, prefix string) []string {
	csvFiles := make([]string, 0)
	walkFunc := func(path string, info os.FileInfo, err error) error {
		if filepath.Ext(path) == ".csv" &&
			prefix != "" &&
			strings.HasPrefix(info.Name(), prefix) {
			csvFiles = append(csvFiles, path)
		}
		return nil
	}
	err := filepath.Walk(dirPath, walkFunc)
	if err != nil {
		log.Fatal(err)
	}
	return csvFiles
}

func main() {
	for _, f := range GetCSVFiles("data/", "Benchmark-Bench") {
		b := NewBenchmark(&f)
		fmt.Println(b.Name)
		fmt.Println(b.MoveFuncName)
		fmt.Println(b.BuildStratName)
		fmt.Println(b.CostFuncName)
		fmt.Println(b.Population)
		b.LoadLastValues()
		fmt.Println(b.StdDev)
		fmt.Println(b.Avg)
		fmt.Println(b.Min)
		fmt.Println(b.Max)
		fmt.Println(b.Variance)
		fmt.Println(b.SumOfDiff)
	}
}
