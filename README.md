# RedditAnswerGenerator
Cobe used to get answer at some subreddit
Use flags to get answer by console and files
## Flags
|Flag name| Param after | Need to |
|----------------|:---------:|:----------------|
-s or --subreddit | subredditName | set the subreddit name to learn/answer 
-l or --learn | none | program will learn and create .brain file
-a or --answer | line answer to | program will find the answer on this string
-ro or --remove-old | none | program delete old .brain files if learn flag checked
-p or --path | pathTo | set the other path to check/put .brain and **_answer.txt file(s)
--retry | retry count | program will retry get answer until he gets the exception and retry count not 0
--logs | none | program will write logs
