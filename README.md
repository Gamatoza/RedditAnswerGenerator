# RedditAnswerGenerator
Cobe used to get answer at some subreddit<br>
Use flags to get answer by console and files
## Flags to MainProject
|Flag name| Param after | Need to |
|----------------|:---------:|:----------------|
-n or --name | subredditName | set the subreddit name to learn/answer 
-l or --learn | none | program will learn and create .brain file
-a or --answer | line answer to | program will find the answer on this string
-ro or --remove-old | none | program delete old .brain files if learn flag checked
-p or --path | pathTo | set the other path to check/put .brain and **_answer.txt file(s)
--retry | retry count | program will retry get answer until he gets the exception and retry count not 0
--logs | none | program will write logs
-ro or --remove-after | none | (on develop) removes the file after learn/answer

## MultyGenerator
If u want just learn brains, add subbreddits (just names!) on subs.txt, and run multygenerator
