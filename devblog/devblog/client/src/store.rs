use serde::{Deserialize, Serialize};
use yewdux::prelude::*;

#[derive(Clone, Serialize, Deserialize, Store, PartialEq, Debug, Default)]
#[store(storage = "local")]
pub struct Store {
    #[serde(default)]
    pub authenticated: bool,
    pub admin: bool,
    pub expiration: String,
    pub token: String,
    pub username: String,
    #[serde(default)]
    pub page_num: i32,
}

#[derive(Clone, Serialize, Deserialize, PartialEq, Debug)]
pub struct PageNum(u32);

impl Default for PageNum {
    fn default() -> Self {
        PageNum(1)
    }
}
