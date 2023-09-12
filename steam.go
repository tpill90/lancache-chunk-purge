package main

import (
	"bufio"
	"crypto/md5"
	"fmt"
	"log"
	"os"
	"os/exec"
	"strings"
)

func main() {
	//1、Extract the last 1000 rows of the access.log
	//2、Change the log to this format：steam/depot/578081/chunk/80b39b8a701aedc383cf2b976eb180b8cd9f0691bytes=0-1048575
	//3、Dedup and Use the last four digits of MD5 to find the file directory and  Delete them
	//The operating is Centos 7
	command := "tail"
	args := []string{"-n", "1000", "/home/lancache/docker-compose-master/lanchache/logs/access.log"}
	outputFile, err := os.Create("tmp.log")
	if err != nil {
		fmt.Println("Can't creat file:", err)
		return
	}
	defer outputFile.Close()
	cmd := exec.Command(command, args...)
	cmd.Stdout = outputFile
	err = cmd.Run()
	if err != nil {
		fmt.Println("Run err:", err)
		return
	}
	filePath := "tmp.log"
	file, err := os.Open(filePath)
	if err != nil {
		fmt.Println("Can't open file:", err)
		return
	}
	var url string
	scanner := bufio.NewScanner(file)
	dedup := make(map[string]bool)
	for scanner.Scan() {
		line := scanner.Text()
		if len(line) >= 2 {
			ss := strings.Split(line, " ")
			for i := range ss {
				if ss[i] == "\"GET" {
					url = ss[i+1]
				}
			}
			sss := strings.Split(url, "?")
			md5str := MD5("steam" + sss[0] + "bytes=0-1048575")
			if !dedup[md5str] {
				dedup[md5str] = true
			} else if dedup[md5str] {
				continue
			}
			lastTwoChars := md5str[len(md5str)-2:]
			lastfouChars := md5str[len(md5str)-4 : len(md5str)-2]
			newPath := "/home/lancache/docker-compose-master/lanchache/cache/cache/" + lastTwoChars + "/" + lastfouChars
			cmd := exec.Command("find", newPath, "-name", md5str, "-exec", "rm", "{}", ";")
			err = cmd.Run()
			if err != nil {
				fmt.Println("Run err:", err)
				return
			}
			fmt.Println("Delete files：", md5str)
		}
	}
	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}
}

func MD5(str string) string {
	data := []byte(str)
	has := md5.Sum(data)
	md5str := fmt.Sprintf("%x", has)
	return md5str
}
