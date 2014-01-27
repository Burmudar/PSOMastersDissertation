package main

import (
	"bufio"
	"encoding/csv"
	"errors"
	"flag"
	"fmt"
	"log"
	"math"
	"os"
	"path/filepath"
	"strconv"
	"strings"
)

const (
	BENCHMARK_PREFIX       = "Benchmark-Bench-"
	KEY_SEP                = "-"
	PER_TRX_CHANNEL_INDEX  = "PerTRXChannelIndexFunction"
	PARTICLE_PER_TRX       = "ParticlePerTrxFunction"
	BUILD_GBEST_FROM_TRX   = "BuildGBestFromTrxs"
	BUILD_GBEST_FROM_CELLS = "BuildGBestFromCells"
	CLONE_IF_GBEST         = "CloneIfGbest"
)

type KeyGenFunc func(b *benchmark) string

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

var KeyGen KeyGenFunc = Key
var benchmarkName string
var moveFuncNum int
var gbestBuildNum int
var analysisFilter int

func Key(b *benchmark) string {
	return b.Name + KEY_SEP + b.MoveFuncName +
		KEY_SEP + b.BuildStratName + KEY_SEP +
		b.CostFuncName + KEY_SEP + strconv.Itoa(b.Population)
}

func KeyNamePopAndMoveFunc(b *benchmark) string {
	return b.Name + KEY_SEP + strconv.Itoa(b.Population) + KEY_SEP + b.MoveFuncName
}

func KeyNamePopAndBuildScheme(b *benchmark) string {
	return b.Name + KEY_SEP + strconv.Itoa(b.Population) + KEY_SEP + b.BuildStratName
}

func KeyBasicNameAndPop(b *benchmark) string {
	return b.Name + KEY_SEP + strconv.Itoa(b.Population)
}

func KeyBasicName(b *benchmark) string {
	return b.Name
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

func (b *benchmark) CopyNamesFrom(other *benchmark) {
	b.File = other.File
	b.Name = other.Name
	b.Population = other.Population
	b.MoveFuncName = other.MoveFuncName
	b.BuildStratName = other.BuildStratName
	b.CostFuncName = other.CostFuncName
}

func (b *benchmark) LoadLastValues() error {
	fileReader := openFile(b.File)
	reader := csv.NewReader(fileReader)
	reader.FieldsPerRecord = 2
	records, error := reader.ReadAll()
	if error != nil {
		log.Fatal(error)
	}
	if len(records) < 54 {
		return errors.New("Benchmark is incomplete - " + b.File)
	}
	b.StdDev = trimAndToFloat(records[51][0])    //std dev
	b.Avg = trimAndToFloat(records[51][1])       //avg
	b.Min = trimAndToFloat(records[52][0])       //min
	b.Max = trimAndToFloat(records[52][1])       //max
	b.Variance = trimAndToFloat(records[53][0])  //variance
	b.SumOfDiff = trimAndToFloat(records[53][1]) //SumOfDiff
	return nil
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

func NewBenchmark(file *string) (*benchmark, error) {
	b := new(benchmark)
	b.File = *file
	_, filename := stripDir(b.File)
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
	error = b.LoadLastValues()
	return b, error
}

func openFile(filename string) *bufio.Reader {
	file, err := os.Open(filename)
	if err != nil {
		log.Fatal(err)
	}
	return bufio.NewReader(file)
}

func wantedBenchmark(filename string) bool {
	switch benchmarkName {
	case "all":
		return true
	case "siemens1":
		return strings.Contains(strings.ToLower(filename), "siemens1")
	case "siemens2":
		return strings.Contains(strings.ToLower(filename), "siemens1")
	case "siemens3":
		return strings.Contains(strings.ToLower(filename), "siemens3")
	case "siemens4":
		return strings.Contains(strings.ToLower(filename), "siemens4")
	default:
		panic("Unkown benchmark option given")
	}
}

func wantedMoveFunc(filename string) bool {
	switch moveFuncNum {
	case 0:
		return true
	case 1:
		return strings.Contains(filename, PARTICLE_PER_TRX)
	case 2:
		return strings.Contains(filename, PER_TRX_CHANNEL_INDEX)
	default:
		panic("Unkown Move Func number given")
	}
}

func wantedGBestScheme(filename string) bool {
	switch gbestBuildNum {
	case 0:
		return true
	case 1:
		return strings.Contains(filename, CLONE_IF_GBEST)
	case 2:
		return strings.Contains(filename, BUILD_GBEST_FROM_CELLS)
	case 3:
		return strings.Contains(filename, BUILD_GBEST_FROM_TRX)
	default:
		panic("Unkown GBest Scheme given")
	}
}

func GetCSVFiles(dirPath, prefix string) []string {
	csvFiles := make([]string, 0)
	walkFunc := func(path string, info os.FileInfo, err error) error {
		if filepath.Ext(path) == ".csv" &&
			prefix != "" &&
			strings.HasPrefix(info.Name(), prefix) {
			if wantedBenchmark(info.Name()) && wantedMoveFunc(info.Name()) && wantedGBestScheme(info.Name()) {
				csvFiles = append(csvFiles, path)
			}
		}
		return nil
	}
	err := filepath.Walk(dirPath, walkFunc)
	if err != nil {
		log.Fatal(err)
	}
	return csvFiles
}

func zip(elements []*benchmark) *benchmark {
	b := new(benchmark)
	b.CopyNamesFrom(elements[0])
	b.Min = elements[0].Min
	var sum float32 = 0.0
	for _, el := range elements {
		sum = sum + el.Min
		if el.Min <= b.Min {
			b.CopyNamesFrom(el)
			b.Min = el.Min
		}
	}
	b.Avg = sum / float32(len(elements))
	var sumOfDiff float32 = 0.0
	for _, el := range elements {
		sumOfDiff = sumOfDiff + (el.Min-b.Avg)*(el.Min-b.Avg)
	}
	b.StdDev = float32(math.Sqrt(float64(sumOfDiff)))
	b.Variance = sumOfDiff / float32(len(elements))
	b.SumOfDiff = sumOfDiff
	return b
}

func getKeyGen(keyType int) KeyGenFunc {
	switch keyType {
	case 0:
		return Key
	case 1:
		return KeyBasicNameAndPop
	case 2:
		return KeyNamePopAndMoveFunc
	case 3:
		return KeyNamePopAndBuildScheme
	case 4:
		return KeyBasicName
	default:
		panic("Unkown Build Scheme option selected")
	}
}

func init() {
	flag.StringVar(&benchmarkName, "Benchmark", "all", "siemens1, siemens2, siemens3, siemens4 ot all")
	flag.IntVar(&moveFuncNum, "MoveFuncNum", 0, "1 - ParticlePerTRX or 2 - PerTRXChannelIndex or 0 for both")
	flag.IntVar(&gbestBuildNum, "GbestBuildScheme", 0, "GBest building scheme: 1 for CloneIfGBest. 2 for BuildGBestFromCells. 3 BuildGBestFromTrxs or 0 for all")
	flag.IntVar(&analysisFilter, "AnalysisFilter", 0, "0 - None, 1 - Best Benchmark regardless of move or gbest scheme, 2 - Best Benchmark with Move method, 3 - Best Benchmark with GBest Scheme, 4 - Best Benchmark regardles of pop, move or gbest scheme")
}

func main() {
	flag.Parse()
	var benchBuckets map[string][]*benchmark = make(map[string][]*benchmark)
	KeyGen = getKeyGen(analysisFilter)
	for _, f := range GetCSVFiles("data/", "Benchmark-Bench") {
		b, error := NewBenchmark(&f)
		if error != nil {
			log.Print(error)
			continue
		}
		key := KeyGen(b)
		bucket, ok := benchBuckets[key]
		if ok {
			bucket = append(bucket, b)
			benchBuckets[key] = bucket
		} else {
			bucket = make([]*benchmark, 1)
			bucket[0] = b
			benchBuckets[key] = bucket
		}
	}
	for key, value := range benchBuckets {
		b := zip(value)
		fmt.Printf("================================================\n")
		fmt.Printf("Key: %v\n", key)
		fmt.Printf("Name: %v\n", b.Name)
		fmt.Printf("Move Function Name: %v\n", b.MoveFuncName)
		fmt.Printf("Cost Function Name: %v\n", b.CostFuncName)
		fmt.Printf("File: %v\n", b.File)
		fmt.Printf("Min: %v\n", b.Min)
		fmt.Printf("Avg: %v\n", b.Avg)
		fmt.Printf("Standard Deviation: %v\n", b.StdDev)
		fmt.Printf("Variance: %v\n", b.Variance)
		fmt.Println("================================================\n")
	}
	fmt.Printf("Completed Analysis with the following flags: -benchmark=%v -moveFuncNum=%v -gbestBuildScheme=%v\n", benchmarkName, moveFuncNum, gbestBuildNum)
}
