use chrono::{NaiveDateTime, Utc};
use serde::{Deserialize, Serialize};

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct YoutubeVideo {
    pub id: u32,
    pub url: String,
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct PostModel {
    pub id: u32,
    pub description: String,
    pub date: NaiveDateTime,
    pub imgs: Vec<ImgModel>,
    pub comments: Vec<CommentModel>,
    #[serde(rename = "upVotes")]
    pub up_votes: Vec<UpVoteModel>,
    #[serde(rename = "downVotes")]
    pub down_votes: Vec<DownVoteModel>,
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct CommentModel {
    pub id: u32,
    #[serde(rename = "postId")]
    pub post_id: u32,
    pub content: String,
    pub date: NaiveDateTime,
    #[serde(rename = "userName")]
    pub username: String,
}

impl CommentModel {
    pub fn new(post_id: u32, content: String, username: String) -> Self {
        CommentModel {
            id: 0,
            post_id,
            content,
            date: Utc::now().naive_utc(),
            username,
        }
    }
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct ImgModel {
    #[serde(rename = "postId")]
    pub post_id: u32,
    pub url: String,
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct UpVoteModel {
    #[serde(rename = "postId")]
    pub post_id: u32,
    #[serde(rename = "userName")]
    pub username: String,
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct DownVoteModel {
    #[serde(rename = "postId")]
    pub post_id: u32,
    #[serde(rename = "userName")]
    pub username: String,
}

#[derive(Serialize, Deserialize)]
pub struct VoteCount {
    pub up: usize,
    pub down: usize,
}

#[derive(Serialize, Deserialize, Default, PartialEq, Clone)]
pub struct Notification {
    #[serde(rename = "postId")]
    pub post_id: u32,
    #[serde(rename = "userName")]
    pub username: String,
    pub author: String,
    #[serde(rename = "imgUrl")]
    pub img_url: String,
    #[serde(rename = "notificationType")]
    pub notification_type: String,
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct IdentityError {
    pub code: String,
    pub description: String,
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug, Default)]
pub struct User {
    #[serde(default, rename = "userName")]
    pub username: String,
    #[serde(default)]
    pub email: String,
    #[serde(default)]
    pub password: String,
    #[serde(default, rename = "passwordHash")]
    pub password_hash: String,
    #[serde(default, rename = "passwordHash")]
    pub password_hash_confirm: String,
    #[serde(default)]
    pub subscribed: bool,
}

impl User {
    pub fn set(mut self, value: String, field: &UserField) -> Self {
        match field {
            UserField::Username => self.username = value,
            UserField::Email => self.email = value,
            UserField::Password => self.password = value,
            UserField::PasswordHash => self.password_hash = value,
            UserField::PasswordHashConfirm => self.password_hash_confirm = value,
        }
        self
    }
}

pub enum UserField {
    Username,
    Password,
    PasswordHash,
    PasswordHashConfirm,
    Email,
}
