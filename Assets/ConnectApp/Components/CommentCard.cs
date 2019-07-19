using System;
using System.Collections.Generic;
using ConnectApp.Constants;
using ConnectApp.Models.Model;
using ConnectApp.Utils;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.widgets;

namespace ConnectApp.Components {
    public class CommentCard : StatelessWidget {
        public CommentCard(
            Message message,
            bool isPraised,
            string parentName = null,
            string parentAuthorId = null,
            GestureTapCallback moreCallBack = null,
            GestureTapCallback praiseCallBack = null,
            GestureTapCallback replyCallBack = null,
            Action<string> pushToUserDetail = null,
            Key key = null
        ) : base(key: key) {
            this.message = message;
            this.isPraised = isPraised;
            this.parentName = parentName;
            this.parentAuthorId = parentAuthorId;
            this.moreCallBack = moreCallBack;
            this.praiseCallBack = praiseCallBack;
            this.replyCallBack = replyCallBack;
            this.pushToUserDetail = pushToUserDetail;
        }

        readonly Message message;
        readonly bool isPraised;
        readonly string parentName;
        readonly string parentAuthorId;
        readonly GestureTapCallback moreCallBack;
        readonly GestureTapCallback praiseCallBack;
        readonly GestureTapCallback replyCallBack;
        readonly Action<string> pushToUserDetail;


        public override Widget build(BuildContext context) {
            if (this.message == null) {
                return new Container();
            }

            var reply = this.message.parentMessageId.isEmpty()
                ? new GestureDetector(
                    onTap: this.replyCallBack,
                    child: new Container(
                        margin: EdgeInsets.only(10),
                        child: new Text(
                            $"回复 {CStringUtils.likeCountToString(likeCount: this.message.replyMessageIds.Count)}",
                            style: CTextStyle.PRegularBody4
                        )
                    )
                )
                : new GestureDetector(
                    child: new Container()
                );
            return new Container(
                color: CColors.White,
                padding: EdgeInsets.only(16, 16, 16),
                child: new Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: new List<Widget> {
                        new GestureDetector(
                            onTap: () => this.pushToUserDetail(this.message.author.id),
                            child: new Container(
                                height: 24,
                                margin: EdgeInsets.only(right: 8),
                                child: Avatar.User(this.message.author, 24)
                            )
                        ),
                        new Expanded(
                            child: new Container(
                                child: new Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: new List<Widget> {
                                        this._buildCommentAvatarName(),
                                        this._buildCommentContent(),

                                        new Row(
                                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                            children: new List<Widget> {
                                                new Text($"{DateConvert.DateStringFromNonce(this.message.nonce)}",
                                                    style: CTextStyle.PSmallBody4),
                                                new Container(
                                                    child: new Row(
                                                        children: new List<Widget> {
                                                            new GestureDetector(
                                                                onTap: this.praiseCallBack,
                                                                child: new Container(
                                                                    color: CColors.White,
                                                                    child: new Text(
                                                                        $"点赞 {CStringUtils.likeCountToString(this.message.reactions.Count)}",
                                                                        style: this.isPraised
                                                                            ? CTextStyle.PRegularBlue
                                                                            : CTextStyle.PRegularBody4
                                                                    )
                                                                )),
                                                            reply
                                                        }
                                                    )
                                                )
                                            }
                                        ),
                                        new Container(
                                            margin: EdgeInsets.only(top: 12),
                                            height: 1,
                                            color: CColors.Separator2
                                        )
                                    }
                                )
                            )
                        )
                    }
                )
            );
        }

        Widget _buildCommentAvatarName() {
            var textStyle = CTextStyle.PMediumBody3.apply(heightFactor: 0, heightDelta: 1);
            return new Container(
                height: 24,
                child: new Row(
                    children: new List<Widget> {
                        new Expanded(
                            child: new GestureDetector(
                                onTap: () => this.pushToUserDetail(this.message.author.id),
                                child: new Text(
                                    data: this.message.author.fullName,
                                    style: textStyle
                                )
                            )
                        ),
                        new CustomButton(
                            padding: EdgeInsets.only(8, 0, 0, 8),
                            onPressed: this.moreCallBack,
                            child: new Icon(
                                Icons.ellipsis,
                                size: 20,
                                color: CColors.BrownGrey
                            )
                        )
                    }
                )
            );
        }

        Widget _buildCommentContent() {
            var content = MessageUtils.AnalyzeMessage(this.message.content, this.message.mentions,
                this.message.mentionEveryone);
            List<TextSpan> textSpans = new List<TextSpan> ();
            if (this.parentName.isNotEmpty()) {
                textSpans.AddRange(new List<TextSpan> {
                    new TextSpan(
                        "回复",
                        style: CTextStyle.PLargeBody4
                    ),
                    new TextSpan(
                        $"@{this.parentName}",
                        style: CTextStyle.PLargeBlue,
                        recognizer: new TapGestureRecognizer {
                            onTap = () => this.pushToUserDetail(this.parentAuthorId)
                        }
                    ),
                    new TextSpan(
                        ": ",
                        style: CTextStyle.PLargeBody
                    )
                });
            }
            textSpans.Add(new TextSpan(
                text: content,
                style: CTextStyle.PLargeBody
            ));
            return new Container(
                margin: EdgeInsets.only(top: 3, bottom: 5),
                child: new RichText(
                    text: new TextSpan(
                        children: textSpans
                    )
                )
            );
        }
    }
}